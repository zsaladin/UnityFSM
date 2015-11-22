using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;

namespace FSM
{
    public class FsmEditor : EditorWindow
    {
        const float INSPECTOR_WIDTH = 300f;
        const float MARGIN = 20f;

        FsmRecord _record;
        FsmRecord.State _currentState;

        Rect _stateArea;
        Rect _inspectorArea;

        System.Type[] _allActionTypes;
        Dictionary<FsmAction, bool> _actionExpandStates = new Dictionary<FsmAction, bool>();
        

        void OnGUI()
        {
            _inspectorArea = new Rect(position.width - INSPECTOR_WIDTH - MARGIN, MARGIN, INSPECTOR_WIDTH, position.height - MARGIN * 2);
            _stateArea = new Rect(MARGIN, MARGIN, _inspectorArea.x - MARGIN * 2, position.height - MARGIN * 2);

            Section_GlobalContextMenu();
            Section_WindowContextMenu();

            //if (_record == null)
            //    return;

            using (UISection.Horizontal.Begin())
            {
                using (UISection.Vertical.Begin(GUILayout.Width(500)))
                {
                    using (UISection.Area.Begin(_stateArea, EditorStyles.helpBox))
                    {
                        Section_StateRects();
                    }
                }

                

                using (UISection.Vertical.Begin(GUILayout.Width(200)))
                {
                    using (UISection.Area.Begin(_inspectorArea, EditorStyles.helpBox))
                    {
                        Section_Inspector();
                    }
                }
                
            }
        }

        public void OnSelectionChange()
        {
            _record = Selection.activeObject as FsmRecord;
            if (_record == null && Selection.activeGameObject != null)
            {
                Fsm fsm = Selection.activeGameObject.GetComponent<Fsm>();
                if (fsm != null)
                    _record = fsm.Record;
            }

            _currentState = null;

            Repaint();
        }

        void OnFocus()
        {
            _allActionTypes = System.Reflection.Assembly.GetAssembly(typeof(FsmAction)).GetTypes().Where(item => item.IsSubclassOf(typeof(FsmAction))).ToArray();
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
            if (_currentState == null || _record == null)
                return;

            using (UISection.Horizontal.Begin())
            {
                GUILayout.Label("Name", GUILayout.Width(100));
                _currentState.Name = GUILayout.TextField(_currentState.Name);
            }

            using (UISection.Vertical.Begin())
            {
                ReorderableListGUI.Title("Actions");
                ReorderableListGUI.ListField(_currentState.Actions, Callback_Action);
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
                if (selected == _currentState)
                    _currentState = null;
            }

        }

        void Menu_AddTransition(object stateObject)
        {

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
            if (e.button == 0 && e.type == EventType.Used)
            {
                FsmRecord.State preState = _currentState;

                _currentState = _record.States[stateIndex];

                if (_currentState != preState)
                {
                    _actionExpandStates.Clear();
                    for (int i = 0; i < _currentState.Actions.Count; ++i)
                    {
                        _actionExpandStates.Add(_currentState.Actions[i], false);
                    }
                }

                GUI.SetNextControlName("");
                GUI.FocusControl("");
                GUI.FocusWindow(id);
            }

        }

        FsmAction Callback_Action(Rect position, FsmAction action)
        {
            Rect btnRect = new Rect(position);
            btnRect.width = 20;

            GUIStyle btnStyle = new GUIStyle(EditorStyles.miniButtonMid);
            btnStyle.fontSize = 15;
            btnStyle.alignment = TextAnchor.MiddleLeft;
            btnStyle.normal.background = null;
           
            if (GUI.Button(btnRect, "▷", btnStyle))
            {
                if (action != null)
                    _actionExpandStates[action] = true;
            }


            Rect popupRect = new Rect(position);
            popupRect.width -= btnRect.width;
            popupRect.x += btnRect.width;
            popupRect.y += 2;

            int preSelectedIndex = action == null ? -1 : ArrayUtility.IndexOf(_allActionTypes, action.GetType());
            int selectedIndex = EditorGUI.Popup(popupRect, preSelectedIndex, _allActionTypes.Select(item => item.ToString()).ToArray());

            if (selectedIndex != preSelectedIndex)
            {
                if (action != null)
                    _actionExpandStates.Remove(action);
                action = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(_allActionTypes[selectedIndex]) as FsmAction;
                _actionExpandStates.Add(action, false);
            }


            return action;
        }
            


        FsmRecord CreateFsmRecord(string path)
        {
            FsmRecord record = CreateInstance<FsmRecord>();
            AssetDatabase.CreateAsset(record, path);
            SaveRecords();

            return record;
        }

        void SaveRecords()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
