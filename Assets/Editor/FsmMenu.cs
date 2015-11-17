using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FSM
{
    public class FsmMenu
    {
        [MenuItem("FSM/Editor")]
        static void OpenFsmEditor()
        {
            EditorWindow.GetWindow<FsmEditor>("FSM Editor").OnSelectionChange();
        }
    }
}
