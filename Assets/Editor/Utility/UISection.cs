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
            if (_editor)
                _editor.EndWindows();
        }

        public static Windows Begin(EditorWindow editor)
        {
            return new Windows(editor);
        }
    }

    public class Horizontal : IDisposable
    {
        private Horizontal(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
        }

        public void Dispose()
        {
            GUILayout.EndHorizontal();
        }

        public static Horizontal Begin(params GUILayoutOption[] options)
        {
            return new Horizontal(options);
        }

    }

    public class Vertical : IDisposable
    {
        private Vertical(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }

        public void Dispose()
        {
            GUILayout.EndVertical();
        }

        public static Vertical Begin(params GUILayoutOption[] options)
        {
            return new Vertical(options);
        }
    }

    public class Area : IDisposable
    {
        private Area(Rect rect, GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
        }

        public void Dispose()
        {
            GUILayout.EndArea();
        }

        public static Area Begin(Rect rect, GUIStyle style)
        {
            return new Area(rect, style);
        }
    }
        
}
