using System.IO;
using UnityEditor;
using UnityEngine;

namespace Cod2Unity
{
    public class SetupWindow : EditorWindow
    {
        static D3DBSP d3dbsp_importer;

        [MenuItem("Cod2Unity/Setup", false, 0)]
        public static void ShowWindow()
        {
            d3dbsp_importer = new D3DBSP();
            SetupWindow window = EditorWindow.GetWindow<SetupWindow>(true, "Cod2Unity Setup");
            window.minSize = new Vector2(380, 664);
        }

        static string installPath = "C:/Program Files (x86)/Activision/Call of Duty 2/";
        Vector2 scrollPosMP;
        Vector2 scrollPosOther;
        bool showMP = true;
        bool showOther = false;
        bool parseValid = false;
        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;

            GUILayout.Label("Setup", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Game Path: ",
                installPath);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Choose Game Install Location"))
            {
                string tempPath = EditorUtility.OpenFolderPanel("Select Installation Location", "", "");
                string[] files = Directory.GetFiles(tempPath);

                bool validGameFolder = false;

                foreach (string file in files)
                    if (file.Contains("CoD2MP_s.exe"))
                        validGameFolder = true;

                if (!validGameFolder)
                {
                    if (EditorUtility.DisplayDialog("Invalid Game Folder",
                       "Game executable not found in the specified installation folder, continue anyway?\n" + "Folder: (" + tempPath + ")"
                       , "Continue", "Cancel"))
                    {
                        validGameFolder = true;
                    }
                }

                if (validGameFolder)
                {
                    installPath = tempPath + "/";
                    parseValid = d3dbsp_importer.ParseGameFolder(installPath);
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                parseValid = d3dbsp_importer.ParseGameFolder(installPath);
            }
            EditorGUILayout.EndHorizontal();

            if (parseValid)
            {
                EditorGUIUtility.labelWidth = 280;
                showMP = EditorGUILayout.BeginFoldoutHeaderGroup(showMP, "Multiplayer Maps");
                if (showMP)
                {
                    EditorGUILayout.BeginVertical();
                    scrollPosMP = EditorGUILayout.BeginScrollView(scrollPosMP, GUILayout.Height(280));

                    foreach (string mapName in Utils.mpMapNames)
                    {
                        EditorGUILayout.BeginHorizontal();
                        string curLabel = System.IO.Path.GetFileNameWithoutExtension(mapName);
                        string curIngameName = Utils.consoleNameToIngameName[curLabel];
                        if (curIngameName != null)
                        {
                            curLabel = curLabel + " (" + curIngameName + ")";
                        }
                        EditorGUILayout.PrefixLabel(curLabel);
                        if (GUILayout.Button("Import", GUILayout.Width(80)))
                        {
                            d3dbsp_importer.LoadMap(mapName);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                showOther = EditorGUILayout.BeginFoldoutHeaderGroup(showOther, "Other Maps");
                if (showOther)
                {
                    EditorGUILayout.BeginVertical();
                    scrollPosOther = EditorGUILayout.BeginScrollView(scrollPosOther, GUILayout.Height(280));

                    foreach (string mapName in Utils.otherMapNames)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(System.IO.Path.GetFileNameWithoutExtension(mapName));
                        if (GUILayout.Button("Import", GUILayout.Width(80)))
                        {
                            d3dbsp_importer.LoadMap(mapName);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            this.Repaint();
        }

        [UnityEditor.MenuItem("Cod2Unity/DebugLoadToujane", false, 11)]
        public static void DebugLoadMap()
        {
            string gamePath = "E:/SteamLibrary/steamapps/common/Call of Duty 2/";
            string mapName = "mp_toujane.d3dbsp";

            d3dbsp_importer = new D3DBSP();
            Utils.ReadZipContentsToDictionaries(gamePath);
            d3dbsp_importer.LoadMap(mapName);
        }
    }
}