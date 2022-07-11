using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

namespace Quartzified.QuickKeep.Editor
{
    public class QuickKeepEditor : EditorWindow
    {
        string packageEntry;
        string keyEntry;
        string valueEntry;

        string debug;

        List<QuickData> data = new List<QuickData>();

        Vector2 scrollPos;


        [MenuItem("Quartzified/QuickKeep/Browser")]
        public static void ShowWindow()
        {
            QuickKeepEditor window = EditorWindow.GetWindow<QuickKeepEditor>("Quick Keep Browser");
            window.minSize = new Vector2(286, 384);
            window.Show();
        }

        [MenuItem("Quartzified/QuickKeep/Show in Explorer")]
        public static void ShowExplorer()
        {
            EditorUtility.RevealInFinder(QuickKeep.DataPath);
        }

        [MenuItem("Quartzified/QuickKeep/Clear All QuickKeep Data")]
        public static void ClearData()
        {
            if(EditorUtility.DisplayDialog("Clear all QuickKeep Data", "You are about to clear all QuickKeep stored data. This action cannot be undone.", "Clear All", "Cancel"))
            {
                QuickKeep.DeleteAll();
            }
        }

        private void OnGUI()
        {
            SearchHeader();

            GUILayout.BeginVertical();

            GUILayout.Space(6);

            if (data.Count > 0)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(Screen.height - 192));
                GUILayout.Label("", GUILayout.Width(274), GUILayout.Height(0));
                GUILayout.Space(6);


                for (int i = 0; i < data.Count; i++)
                {
                    GUILayout.Label(string.Format("{0} = {1}", data[i].key, data[i].value));
                }


                GUI.EndScrollView();
            }

            GUILayout.EndVertical();


            DebugFooter();
        }

        void SearchHeader()
        {
            GUIStyle style = new GUIStyle();
            style.richText = true;

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label("<color=white><b>Package</b></color>", style, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            packageEntry = GUILayout.TextField(packageEntry, GUILayout.Width(280));

            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label("<color=white><b>Key</b></color>", style, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            keyEntry = GUILayout.TextField(keyEntry, GUILayout.Width(280));

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label("<color=white><b>Value</b></color>", style, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            valueEntry = GUILayout.TextField(valueEntry, GUILayout.Width(280));

            GUILayout.Space(6);

            GUILayout.BeginHorizontal();

            GUILayout.Space(12);

            if (GUILayout.Button("Get", GUILayout.Width(123)))
            {
                if (string.IsNullOrEmpty(packageEntry))
                    packageEntry = "global";

                debug = string.Format("Getting {0} from {1}...", keyEntry, packageEntry);
                Get(packageEntry, keyEntry);
            }

            GUILayout.Space(12);

            if (GUILayout.Button("Set", GUILayout.Width(123)))
            {
                debug = string.Format("Setting {0} in {1}...", keyEntry, packageEntry);
                Set(keyEntry, valueEntry, packageEntry);
            }

            GUILayout.Space(12);

            GUILayout.EndHorizontal();
        }

        void DebugFooter()
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUIStyle style = new GUIStyle();
            style.richText = true;
            GUILayout.Label("<color=white><b>" + debug + "</b></color>", style);
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
        }

        List<QuickData> Get(string package = "", string key = "")
        {
            if (string.IsNullOrEmpty(package))
                package = "global";

            data.Clear();

            if(File.Exists(QuickKeep.GetDirectory(package)))
            {
                string path = QuickKeep.GetDirectory(package);
                List<string> line = File.ReadAllLines(path).ToList();

                if (string.IsNullOrEmpty(key))
                {
                    for (int i = 0; i < line.Count; i++)
                    {
                        string[] lineData = line[i].Split('=');

                        QuickData quickData = new QuickData(lineData[0], lineData[1]);
                        data.Add(quickData);
                    }
                    debug = string.Format("Retrieved all entries from {0}", packageEntry);
                    return data;
                }
                else
                {
                    for (int i = 0; i < line.Count; i++)
                    {
                        string[] lineData = line[i].Split('=');

                        if (lineData[0].Equals(key))
                        {
                            QuickData quickData = new QuickData(lineData[0], lineData[1]);
                            data.Add(quickData);
                            debug = string.Format("Retrieved Key: {0} from {1}", keyEntry, packageEntry);
                            return data;
                        }
                    }

                    debug = string.Format("No key {0} found in {1}", keyEntry, packageEntry);
                    return data;
                }
            }

            debug = string.Format("No Package: {0} could be found!", packageEntry);
            return data;
        }

        void Set(string key, string value, string package = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                debug = string.Format("Set failed: <color=red>key</color> is not set");
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                debug = string.Format("Set failed: <color=red>value</color> is not set");
                return;
            }

            if (string.IsNullOrEmpty(package))
                package = "global";

            if (!Directory.Exists(QuickKeep.DataPath))
                Directory.CreateDirectory(QuickKeep.DataPath);

            string path = QuickKeep.GetDirectory(package);

            List<QuickData> writeData = Get(package);

            List<string> lines = new List<string>();

            if(writeData.Count > 0)
            {
                bool overWritten = false;
                for (int i = 0; i < writeData.Count; i++)
                {
                    if (writeData[i].key.Equals(key))
                    {
                        lines.Add(string.Format("{0}={1}", key, value));
                        overWritten = true;
                    }
                    else
                    {
                        lines.Add(string.Format("{0}={1}", writeData[i].key, writeData[i].value));
                    }
                }

                if(!overWritten)
                {
                    lines.Add(string.Format("{0}={1}", key, value));
                }

            }
            else
            {
                lines.Add(string.Format("{0}={1}", key, value));
            }

            File.WriteAllLines(path, lines);

            debug = string.Format("Succesfully set key entry!");
        }

        [System.Serializable]
        class QuickData
        {
            public string key;
            public string value;

            public QuickData(string _key, string _value)
            {
                key = _key;
                value = _value;
            }
        }
    }

}

