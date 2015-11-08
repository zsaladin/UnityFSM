using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FSM
{
    public class FsmEditor : EditorWindow
    {
        void OnGUI()
        {
            if (GUILayout.Button("Create"))
            {
                FsmRecord record = ScriptableObject.CreateInstance<FsmRecord>();
                string path = "Assets/Resources/Fsm/record.asset";
                AssetDatabase.CreateAsset(record, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
            }

            using (UISection.Windows.Begin(this))
            {

            }

        }
    }
}
