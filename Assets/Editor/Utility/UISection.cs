using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public static class UISection
{
    public class Windows : IDisposable
    {
        EditorWindow _editor;
        private Windows(EditorWindow editor)
        {
            _editor = editor;
            _editor.BeginWindows();
        }

        public void Dispose()
        {
            if (_editor != null)
                _editor.EndWindows();
        }

        public static Windows Begin(EditorWindow editor)
        {
            return new Windows(editor);
        }
    }
}
