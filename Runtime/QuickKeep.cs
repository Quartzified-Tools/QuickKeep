using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Quartzified.QuickKeep.Data;

namespace Quartzified.QuickKeep
{
    public static class QuickKeep
    {
        #region Set

        /// <summary>
        /// Sets the string (value) to the specified (key)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="package">Data Package</param>
        public static void SetString(string key, string value, string package = "") => SetData.SetString(key, value, package);

        /// <summary>
        /// Sets the int (value) to the specified (key)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="package">Data Package</param>
        public static void SetInt(string key, int value, string package = "") => SetData.SetInt(key, value, package);

        /// <summary>
        /// Sets the float (value) to the specified (key)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="package">Data Package</param>
        public static void SetFloat(string key, float value, string package = "") => SetData.SetFloat(key, value, package);

        /// <summary>
        /// Sets the double (value) to the specified (key)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="package">Data Package</param>
        public static void SetDouble(string key, double value, string package = "") => SetData.SetDouble(key, value, package);

        /// <summary>
        /// Sets the bool (value) to the specified (key)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="package">Data Package</param>
        public static void SetBool(string key, bool value, string package = "") => SetData.SetBool(key, value, package);

        /// <summary>
        /// Sets the DateTime (value) to the specified (key)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="package">Data Package</param>
        public static void SetDateTime(string key, DateTime value, string package = "") => SetData.SetDateTime(key, value, package);

        #endregion

        #region Get

        public static string GetString(string key, string package = "") => GetData.GetString(key, package);

        public static int GetInt(string key, string package = "", int defaultValue = 0) => GetData.GetInt(key, package, defaultValue);

        public static float GetFloat(string key, string package = "", float defaultValue = 0.0f) => GetData.GetFloat(key, package, defaultValue);

        public static double GetDouble(string key, string package = "", double defaultValue = 0.0) => GetData.GetDouble(key, package, defaultValue);

        public static bool GetBool(string key, string package = "", bool defaultValue = false) => GetData.GetBool(key, package, defaultValue);

        public static DateTime GetDateTime(string key, string package = "", DateTime defaultValue = new DateTime()) => GetData.GetDateTime(key, package, defaultValue);

        #endregion

        public static bool HasKey(string key, string package = "")
        {
            if (string.IsNullOrEmpty(package))
                package = "global";

            if (SetData.packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = SetData.packages[package];

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
            string value = GetString(key, package);

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
            if (SetData.packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = SetData.packages[package];

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

                    SetData.packages.Remove(package);
                }
            }
        }

        public static void SaveAll(bool keepTemp = true)
        {
            foreach (KeyValuePair<string, List<QuickData>> item in SetData.packages)
            {
                SavePackage(item.Key, keepTemp);
            }
        }

        #endregion

        #region Delete

        public static void DeleteTemp() => SetData.packages.Clear();

        public static void DeleteKey(string key, string package = "", bool keepTemp = false)
        {
            if (SetData.packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = SetData.packages[package];

                // Check if key entry already exists
                QuickData tempData = tempDataList.Find(d => d.key == key);

                // Remove Entry if Exists
                if (tempData != null)
                {
                    tempDataList.RemoveAt(tempDataList.IndexOf(tempData));
                }

                SetData.packages[package] = tempDataList;
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
            if (SetData.packages.ContainsKey(package))
            {
                SetData.packages.Remove(package);
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



        public static string GetDirectory(string package = "")
        {
            if (!string.IsNullOrEmpty(package))
                return string.Format("{0}/{1}.qk", DataPath, package);

            return string.Format("{0}/global.qk", DataPath);

        }

        public static string DataPath => string.Format("{0}/{1}/", Application.persistentDataPath, "QuickKeep");
    }

}

