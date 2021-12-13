using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cod2Unity
{
    public class AboutWindow : EditorWindow
    {
        [MenuItem("Cod2Unity/About", false, 9)]
        public static void ShowWindow()
        {
            AboutWindow window = EditorWindow.GetWindow<AboutWindow>(true, "About Cod2Unity");
            window.minSize = new Vector2(330, 96);

            m_LinkStyle = new GUIStyle(EditorStyles.label);
            m_LinkStyle.wordWrap = false;
            // Match selection color which works nicely for both light and dark skins
            m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            m_LinkStyle.stretchWidth = false;
        }

        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;
            GUILayout.Label("Cod2Unity - Call of Duty 2 Map Loader for Unity", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Author: ");
            if (GUILayout.Button("Cem Aslan", m_LinkStyle))
            {
                Application.OpenURL("https://iamaslan.com/");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("GitHub Repository: ");
            if (GUILayout.Button("github.com/buzdolapci/Cod2Unity", m_LinkStyle))
            {
                Application.OpenURL("https://github.com/buzdolapci/Cod2Unity");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Thanks to: ");
            if (GUILayout.Button("CptAsgard,", m_LinkStyle))
            {
                Application.OpenURL("https://github.com/CptAsgard");
            }
            if (GUILayout.Button("CoDEmanX, Daevius,", m_LinkStyle))
            {
                Application.OpenURL("https://wiki.zeroy.com/index.php?title=Call_of_Duty_2:_d3dbsp");
            }
            if (GUILayout.Button("peti1106,", m_LinkStyle))
            {
                Application.OpenURL("https://github.com/peti1106");
            }
            if (GUILayout.Button("nonsensation", m_LinkStyle))
            {
                Application.OpenURL("https://github.com/nonsensation");
            }
            EditorGUILayout.EndHorizontal();

            this.Repaint();
        }
        GUIStyle LinkStyle
        {
            get
            {
                return m_LinkStyle;
            }
        }

        static GUIStyle m_LinkStyle;
        bool LinkLabel(GUIContent label/*, params GUILayoutOption[] options*/)
        {
            var position = GUILayoutUtility.GetRect(label, LinkStyle/*, options*/);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }


    }
}