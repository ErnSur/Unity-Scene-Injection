using System;
using QuickEye.SceneInjection.FractionalIndexing;
using UnityEditor;
using UnityEngine;

namespace QuickEye.SceneInjection
{
    internal class DebugWindow : EditorWindow
    {
        [SerializeField]
        private string inputA, inputB;

        [SerializeField]
        private string digitsBase10 = "0123456789";

        [SerializeField]
        private bool useBase62;
        
        [MenuItem("DEBUG/DebugWindow")]
        public static void Open()
        {
            GetWindow<DebugWindow>();
        }
        
        
        private void OnGUI()
        {
            useBase62 = EditorGUILayout.Toggle("Use Base 62",useBase62);
            using (new EditorGUILayout.HorizontalScope())
            {
                inputA = InputField(inputA);
                inputB = InputField(inputB);
            }

            try
            {
                var digits = useBase62 ? OrderKey.Base62Digits : digitsBase10;
                GUILayout.Label(OrderKey.Midpoint(inputA,inputB,digits));
            }
            catch (Exception e)
            {
                GUILayout.Label(e.Message);
            }
        }

        private string InputField(string input)
        {
            var v = EditorGUILayout.TextField(input);
            if (v == "[") return "";
            if (v == "]") return null;
            return v;
        }
    }
}