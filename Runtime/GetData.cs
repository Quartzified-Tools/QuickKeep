using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quartzified.QuickKeep.Data
{
    public static class GetData
    {
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
                switch (int.Parse(value))
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

            if (SetData.packages.ContainsKey(package))
            {
                // Get data list from temp entry
                List<QuickData> tempDataList = SetData.packages[package];

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
    }
}