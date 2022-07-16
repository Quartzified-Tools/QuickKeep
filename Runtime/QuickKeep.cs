using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace Quartzified.QuickKeep
{
    public static class QuickKeep
    {
        [Serializable]
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

        static Dictionary<string, List<QuickData>> packages = new Dictionary<string, List<QuickData>>();

        #region Set

        public static void SetString(string key, string value, string package = "")
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (string.IsNullOrEmpty(value))
                return;

            QuickData data = new QuickData(key, value);

            WriteTempData(data, package);
        }

        public static void SetInt(string key, int value, string package = "")
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetFloat(string key, float value, string package = "")
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetBool(string key, bool value, string package = "")
        {
            if (string.IsNullOrEmpty(key))
                return;

            int writeValue = value == true ? 1 : 0;

            QuickData data = new QuickData(key, writeValue.ToString());

            WriteTempData(data, package);
        }

        public static void SetDateTime(string key, DateTime value, string package = "")
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString("O"));

            WriteTempData(data, package);
        }

        #endregion

        #region Get

        public static string GetString(string key, string package = "") => Get(key, package);

        public static int GetInt(string key, string package = "", int defaultValue = 0)
        {
            string value = Get(key, package);

            if (!string.IsNullOrEmpty(value))
            {
                return int.Parse(value);
            }

            return defaultValue;
        }

        public static float GetFloat(string key, string package = "", float defaultValue = 0f)
        {
            string value = Get(key, package);

            if (!string.IsNullOrEmpty(value))
            {
                return float.Parse(value);
            }

            return defaultValue;
        }

        public static bool GetBool(string key, string package = "", bool defaultValue = false)
        {
            string value = Get(key, package);

            if (!string.IsNullOrEmpty(value))
            {
                switch(int.Parse(value))
                {
                    case 1:
                        return true;
                    case 0:
                        return false;
                }
            }

            return defaultValue;
        }

        public static DateTime GetDateTime(string key, string package = "", DateTime defaultValue = new DateTime())
        {
            string value = Get(key, package);

            if (!string.IsNullOrEmpty(value))
            {
                return DateTime.ParseExact(value, "O", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
            }

            return defaultValue;
        }

        static string Get(string key, string package = "")
        {
            if (string.IsNullOrEmpty(package))
                package = "global";

            if (packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = packages[package];

                // Check if key entry already exists
                QuickData tempData = tempDataList.Find(d => d.key == key);

                // Key Exists
                if (tempData != null)
                {
                    return tempData.value;
                }
            }
            else if (File.Exists(GetDirectory(package)))
            {
                string path = GetDirectory(package);
                List<string> line = File.ReadAllLines(path).ToList();

                for (int i = 0; i < line.Count; i++)
                {
                    string[] lineData = line[i].Split('=');

                    if (lineData[0].Equals(key))
                    {
                        return lineData[1];
                    }
                }
            }

            return null;
        }

        #endregion

        public static bool HasKey(string key, string package = "")
        {
            if (string.IsNullOrEmpty(package))
                package = "global";

            if (packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = packages[package];

                // Check if key entry already exists
                QuickData tempData = tempDataList.Find(d => d.key.Equals(key));

                // Key Exists
                if (tempData != null)
                {
                    return true;
                }
            }
            else if (File.Exists(GetDirectory(package)))
            {
                string path = GetDirectory(package);
                List<string> line = File.ReadAllLines(path).ToList();

                for (int i = 0; i < line.Count; i++)
                {
                    string[] lineData = line[i].Split('=');

                    if (lineData[0].Equals(key))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #region Save

        public static void SaveKey(string key, string package = "")
        {
            string value = Get(key, package);

            if (!string.IsNullOrEmpty(value))
            {
                string path = GetDirectory(package);
                List<string> lines = File.ReadAllLines(path).ToList();

                for (int i = 0; i < lines.Count; i++)
                {
                    string[] lineData = lines[i].Split('=');

                    if (lineData[0].Equals(key))
                    {
                        lines[i] = string.Format("{0}={1}", key, value);
                        break;
                    }
                }

                File.WriteAllLines(path, lines);
            }
        }

        public static void SavePackage(string package = "", bool keepTemp = true)
        {
            List<string> lines = new List<string>();

            // Check if temp entry already exists
            if (packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = packages[package];

                for (int i = 0; i < tempDataList.Count; i++)
                {
                    lines.Add(string.Format("{0}={1}", tempDataList[i].key, tempDataList[i].value));
                }

                File.WriteAllLines(GetDirectory(package), lines);

                // Check if keeptemp data => False to delete
                if (!keepTemp)
                {
                    if (string.IsNullOrEmpty(package))
                        package = "global";

                    packages.Remove(package);
                }
            }
        }

        public static void SaveAll(bool keepTemp = true)
        {
            foreach (KeyValuePair<string, List<QuickData>> item in packages)
            {
                SavePackage(item.Key, keepTemp);
            }
        }

        #endregion

        #region Delete

        public static void DeleteTemp() => packages.Clear();

        public static void DeleteKey(string key, string package = "", bool keepTemp = false)
        {
            if (packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = packages[package];

                // Check if key entry already exists
                QuickData tempData = tempDataList.Find(d => d.key == key);

                // Remove Entry if Exists
                if (tempData != null)
                {
                    tempDataList.RemoveAt(tempDataList.IndexOf(tempData));
                }

                packages[package] = tempDataList;
            }

            if (!keepTemp)
            {
                if (File.Exists(GetDirectory(package)))
                {
                    string path = GetDirectory(package);
                    List<string> line = File.ReadAllLines(path).ToList();

                    for (int i = 0; i < line.Count; i++)
                    {
                        string[] lineData = line[i].Split('=');

                        if (lineData[0].Equals(key))
                        {
                            line.RemoveAt(i);
                        }
                    }

                    File.WriteAllLines(path, line);
                }
            }
        }

        public static void DeletePackage(string package = "", bool keepTemp = false)
        {
            if (packages.ContainsKey(package))
            {
                packages.Remove(package);
            }

            if (!keepTemp)
            {
                File.Delete(GetDirectory(package));
            }
        }

        public static void DeleteAll()
        {
            DirectoryInfo di = new DirectoryInfo(DataPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            di.Delete();
        }

        #endregion

        static void WriteTempData(QuickData data, string package = "")
        {
            if (string.IsNullOrEmpty(package))
                package = "global";

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

            // Check if temp entry already exists
            if (packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = packages[package];

                // Check if key entry already exists
                QuickData tempData = tempDataList.Find(d => d.key == data.key);

                // Update key entry
                if (tempData != null)
                {
                    tempData.value = data.value;
                    tempDataList[tempDataList.IndexOf(tempData)] = tempData;
                }
                else // Add new Key entry
                {
                    tempDataList.Add(data);
                }

                packages[package] = tempDataList;
            }
            else
            {
                // Create new data list
                List<QuickData> tempDataList = new List<QuickData>();

                // See if data file already exists and load all data
                if (File.Exists(GetDirectory(package)))
                {
                    List<string> file = File.ReadAllLines(GetDirectory(package)).ToList();

                    for (int i = 0; i < file.Count; i++)
                    {
                        string[] newData = file[i].Split('=');
                        tempDataList.Add(new QuickData(newData[0], newData[1]));
                    }
                }

                // Check if key entry already exists
                QuickData tempData = tempDataList.Find(d => d.key == data.key);

                // Update key entry
                if (tempData != null)
                {
                    tempData.value = data.value;
                    tempDataList[tempDataList.IndexOf(tempData)] = tempData;
                }
                else // Add new Key entry
                {
                    tempDataList.Add(data);
                }

                packages.Add(package, tempDataList);
            }
        }

        public static string GetDirectory(string package = "")
        {
            if (!string.IsNullOrEmpty(package))
                return string.Format("{0}/{1}.qk", DataPath, package);

            return string.Format("{0}/global.qk", DataPath);

        }

        public static string DataPath => string.Format("{0}/{1}/", Application.persistentDataPath, "QuickKeep");
    }

}

