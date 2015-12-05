using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace FSM
{
    public class FsmEditor : EditorWindow
    {
        const float INSPECTOR_WIDTH = 230f;
        const float MARGIN = 2f;

        string fullFileName;
        FsmRecord _record;

        FsmRecord.State _currentState;
        FsmRecord.State CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                RefreshActionReorderList();
                RefreshTransitionReorderList();
            }
        }

        Rect _stateArea;
        Rect _inspectorArea;
        Rect _inspectorDetailArea;

        System.Type[] _allActionTypes;
        Dictionary<FsmAction, bool> _actionExpandStates = new Dictionary<FsmAction, bool>();
        ReorderableList _actionReorderableList;

        ReorderableList _transitionReorderableList;

        FsmRecord.State _transitionStartState;

        object _currentDetail;
        

        void OnGUI()
        {
            _inspectorDetailArea = new Rect(position.width - INSPECTOR_WIDTH - MARGIN, MARGIN, INSPECTOR_WIDTH, position.height - MARGIN * 2);
            _inspectorArea = new Rect(_inspectorDetailArea.x - INSPECTOR_WIDTH - MARGIN, MARGIN, INSPECTOR_WIDTH, position.height - MARGIN * 2);
            _stateArea = new Rect(MARGIN, MARGIN, _inspectorArea.x - MARGIN * 2, position.height - MARGIN * 2);

            Section_GlobalContextMenu();
            Section_WindowContextMenu();

            using (UISection.Horizontal.Begin())
            {
                using (UISection.Vertical.Begin())
                {
                    using (UISection.Area.Begin(_stateArea, EditorStyles.helpBox))
                    {
                        Section_StateRects();
                    }
                }

                using (UISection.Vertical.Begin())
                {
                    using (UISection.Area.Begin(_inspectorArea, EditorStyles.helpBox))
                    {
                        Section_Inspector();
                    }
                }

                using (UISection.Vertical.Begin())
                {
                    using (UISection.Area.Begin(_inspectorDetailArea, EditorStyles.helpBox))
                    {
                        Section_InspectorDetail();
                    }
                }
                
            }

            Update_OnTransitioning();
        }

        public void OnSelectionChange()
        {
            Selection.pa
            try
            {
                BinaryFormatter fomatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(fullFileName, FileAccess.ReadWrite))
                {

                }
            }
            catch
            {

            }
            _record = Selection.activeObject as FsmRecord;
            if (_record == null && Selection.activeGameObject != null)
            {
                Fsm fsm = Selection.activeGameObject.GetComponent<Fsm>();
                if (fsm != null)
                    _record = fsm.Record;
            }

            CurrentState = null;

            Repaint();
        }

        void OnFocus()
        {
            _allActionTypes = Assembly.GetAssembly(typeof(FsmAction)).GetTypes().Where(item => item.IsSubclassOf(typeof(FsmAction))).ToArray();

            RefreshActionReorderList();
            RefreshTransitionReorderList();
        }

        void Section_GlobalContextMenu()
        {
            if (Event.current == null || Event.current.type != EventType.ContextClick)
                return;

            if (_stateArea.Contains(Event.current.mousePosition) == false)
                return;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create new FSM"), false, Menu_CreateNewFSM);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add state"), false, Menu_AddState, Event.current.mousePosition);
            menu.AddItem(new GUIContent("Add state from existing"), false, null);
            menu.ShowAsContext();

            Event.current.Use();
                
        }

        void Section_WindowContextMenu()
        {
            if (Event.current == null || Event.current.type != EventType.MouseUp || Event.current.button != 1)
                return;

            FsmRecord.State state = _record.States.Find(item => item.Rect.Contains(Event.current.mousePosition));
            if (state == null)
                return;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add transition"), false, Menu_AddTransition, state);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove state"), false, Menu_RemoveState, state);
            menu.ShowAsContext();

            Event.current.Use();
        }

        void Section_StateRects()
        {
            if (_record != null)
            {
                using (UISection.Windows.Begin(this))
                {
                    for (int i = 0; i < _record.States.Count; ++i)
                    {
                        FsmRecord.State state = _record.States[i];
                        state.Rect = GUI.Window(i + 1, state.Rect, Callback_Window, state.Name);
                    }
                }
            }
            else
            {
                GUIStyle centeredStyle =  new GUIStyle(GUI.skin.label);
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 20;
                centeredStyle.fontStyle = FontStyle.BoldAndItalic;
                centeredStyle.normal.textColor = Color.grey;
                GUI.Label(new Rect(0, 0, _stateArea.width, _stateArea.height), "Create FsmRecord or Select FsmRecord in project view", centeredStyle); 
            }
        }

        void Section_Inspector()
        {
            if (CurrentState == null || _record == null)
                return;

            using (UISection.Horizontal.Begin())
            {
                GUILayout.Label("Name", GUILayout.Width(100));
                CurrentState.Name = GUILayout.TextField(CurrentState.Name);
            }

            
            _actionReorderableList.DoLayoutList();
            _transitionReorderableList.DoLayoutList();
        }

        void Section_InspectorDetail()
        {
            if (_record == null)
                return;

            if (GUILayout.Button("Save", EditorStyles.miniButton))
            {
                SaveRecords();
            }

            if (CurrentState == null || _currentDetail == null)
                return;

            GUILayout.Space(5f);

            FsmAction action = _currentDetail as FsmAction;
            if (action != null)
            {
                Type actionType = action.GetType();
                FieldInfo[] fields = actionType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; ++i)
                {
                    // attribute로 체크 하자
                    FieldInfo field = fields[i];
                    object value = field.GetValue(action);
                    using (UISection.Horizontal.Begin())
                    {
                        EditorGUILayout.LabelField(field.Name, GUILayout.Width(100));
                        if (value is int)
                        {
                            int preValue = (int)value;
                            int newValue = EditorGUILayout.IntField(preValue);
                            if (preValue != newValue)
                            {
                                field.SetValue(action, newValue);
                            }
                        }
                    }
                }
            }

        }

        void Menu_CreateNewFSM()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save your fsm record", "FsmRecord.asset", "asset", "", "Assets/Resources/Fsm");
            _record = CreateFsmRecord(path);
            EditorGUIUtility.PingObject(_record);
            Selection.activeObject = _record;
            EditorUtility.FocusProjectWindow();
        }

        void Menu_AddState(object userData)
        {
            if (_record == null)
                return;

            Vector2 pos = (Vector2)userData;
            FsmRecord.State state = new FsmRecord.State();
            state.Rect = new Rect(pos, new Vector2(100, 60));
            _record.States.Add(state);

            if (_record.States.Count == 1)
            {
                _record.EntryState = state;
            }

            SaveRecords();
            Repaint();
        }

        void Menu_RemoveState(object stateObject)
        {
            FsmRecord.State selected = stateObject as FsmRecord.State;
            if (selected != null)
            {
                _record.States.Remove(selected);
                if (selected == CurrentState)
                    CurrentState = null;
            }

        }

        void Menu_AddTransition(object stateObject)
        {
            if (CurrentState != null)
            {
                 _transitionStartState = CurrentState;
            }
        }

        

        void Callback_Window(int id)
        {
            int stateIndex = id - 1;
            FsmRecord.State thisWindowState = _record.States[stateIndex];
            for (int i = 0; i < thisWindowState.Actions.Count; ++i)
            {
                if (thisWindowState.Actions[i] != null)
                    GUILayout.Label(thisWindowState.Actions[i].GetType().Name.Replace("FsmAction_", ""));
            }

            GUI.DragWindow();

            Event e = Event.current;
            if (e.type == EventType.Used)
            {
                FsmRecord.State preState = CurrentState;

                CurrentState = _record.States[stateIndex];

                if (CurrentState != preState)
                {
                    _actionExpandStates.Clear();
                    for (int i = 0; i < CurrentState.Actions.Count; ++i)
                    {
                        _actionExpandStates.Add(CurrentState.Actions[i], false);
                    }
                }

                GUI.SetNextControlName("");
                GUI.FocusControl("");
                GUI.FocusWindow(id);
            }

        }
            
        void Update_OnTransitioning()
        {
            if (CurrentState == null)
                _transitionStartState = null;

            if (Event.current.type == EventType.Used && _transitionStartState != null)
            {
                FsmRecord.State selected = _record.States.Find(item => item.Rect.Contains(Event.current.mousePosition));
                if (selected != null && selected != _transitionStartState)
                {
                    //_transitionStartState.Transitions.Add(new FsmTransition { NextState = selected } );
                    Debug.Log(selected.Name);
                }

                _transitionStartState = null;
            }

            if (_transitionStartState == null)
                return;



            Color original = Handles.color;
            Handles.color = Color.black;
            //Handles.DrawLine(_currentState.Rect.center, Event.current.mousePosition);

            Vector3 dir = Event.current.mousePosition - CurrentState.Rect.center;
            Handles.Button(CurrentState.Rect.center, Quaternion.identity , dir.magnitude , dir.magnitude * 0.5f, Handles.CubeCap);
            Handles.color = original;

            Repaint();
        }

        FsmRecord CreateFsmRecord(string path)
        {
            path = Application.dataPath + path;

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, _record);
            }

                FsmRecord record = CreateInstance<FsmRecord>();
            SaveRecords();

            return record;
        }

        void SaveRecords()
        {
            AssetDatabase.SaveAssets(); 
            AssetDatabase.Refresh();
        }

        void RefreshActionReorderList()
        {
            if (CurrentState == null)
                return;

            _actionReorderableList = new ReorderableList(CurrentState.Actions, typeof(FsmAction), true, true, true, true);
            _actionReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Actions");
            };

            _actionReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                rect.width -= 26;
                FsmAction action = CurrentState.Actions[index];
                List<string> typestring = _allActionTypes.Select(item => item.ToString()).ToList();
                int preIndex = typestring.FindIndex(item => item == action.ToString());
                int curIndex = EditorGUI.Popup(rect, preIndex, typestring.ToArray());
                if (preIndex != curIndex)
                {
                    CurrentState.Actions[index] = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(_allActionTypes[curIndex] as System.Type) as FsmAction;
                }

                rect.x += rect.width + 1;
                rect.width = 25;

                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fontSize = 15;
                style.fixedHeight = 15;
                style.alignment = TextAnchor.MiddleCenter;

                if (GUI.Button(rect, "▷", style))
                {
                    _currentDetail = action;
                }

            };

            _actionReorderableList.onAddDropdownCallback = (rect, list) =>
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < _allActionTypes.Length; ++i)
                    menu.AddItem(new GUIContent(_allActionTypes[i].ToString()), false, (obj) =>
                    {
                        FsmAction action = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(obj as System.Type) as FsmAction;
                        CurrentState.Actions.Add(action);
                    }, _allActionTypes[i]);
                menu.ShowAsContext();
            };

            _actionReorderableList.onRemoveCallback = (list) =>
            {
                if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the action?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
        }

        void RefreshTransitionReorderList()
        {
            if (CurrentState == null)
                return;

            _transitionReorderableList = new ReorderableList(CurrentState.Transitions, typeof(FsmTransition));
            _transitionReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Transitions");
            };
        }
    }
}
