using UnityEngine;
using UnityEditor;
using System.Collections;

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
        

        void OnGUI()
        {
            _inspectorArea = new Rect(position.width - INSPECTOR_WIDTH - MARGIN, MARGIN, INSPECTOR_WIDTH, position.height - MARGIN * 2);
            _stateArea = new Rect(MARGIN, MARGIN, _inspectorArea.x - MARGIN * 2, position.height - MARGIN * 2);

            Section_ContextMenu();

            if (_record == null)
                return;

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

            Update_WindowSelecting();
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

        void Update_WindowSelecting()
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown)
                return;

            if (e.button != 0)
                return;

            //for (int i = 0; i < _record.States.Count; ++i)
            //{
            //    FsmRecord.State state = _record.States[i];
            //    if (state.Rect.Contains(e.mousePosition))
            //    {
            //        GUI.FocusWindow(i + 1);
            //        break;
            //    }

            //}
        }

        void Section_ContextMenu()
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
                
        }

        void Section_Inspector()
        {
            if (_currentState == null)
                return;

            using (UISection.Horizontal.Begin())
            {
                GUILayout.Label("Name", GUILayout.Width(100));
                _currentState.Name = GUILayout.TextField(_currentState.Name);
            }

            using (UISection.Horizontal.Begin())
            {
                GUILayout.Label("Actions", GUILayout.Width(100));
                _currentState.Name = GUILayout.TextField(_currentState.Name);
            }
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
                        state.Rect = GUI.Window(i + 1, state.Rect, Window_Callback, state.Name);
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

        void Window_Callback(int id)
        {
            GUI.DragWindow();
            Event e = Event.current;

            if (e.button != 0)
                return;

            if (e.type != EventType.Used)
                return;

            int stateIndex = id - 1;
            _currentState = _record.States[stateIndex];

            GUI.FocusControl("");
            GUI.SetNextControlName("");
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
