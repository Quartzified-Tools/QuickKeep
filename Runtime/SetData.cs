using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quartzified.QuickKeep.Data
{
    public static class SetData
    {
        public static Dictionary<string, List<QuickData>> packages = new Dictionary<string, List<QuickData>>();

        public static void SetString(string key, string value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (string.IsNullOrEmpty(value))
                return;

            QuickData data = new QuickData(key, value);

            WriteTempData(data, package);
        }

        public static void SetByte(string key, byte value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetShort(string key, short value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetInt(string key, int value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetLong(string key, long value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetFloat(string key, float value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetDouble(string key, double value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString());

            WriteTempData(data, package);
        }

        public static void SetBool(string key, bool value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            int writeValue = value == true ? 1 : 0;

            QuickData data = new QuickData(key, writeValue.ToString());

            WriteTempData(data, package);
        }

        public static void SetDateTime(string key, DateTime value, string package)
        {
            if (string.IsNullOrEmpty(key))
                return;

            QuickData data = new QuickData(key, value.ToString("O"));

            WriteTempData(data, package);
        }

        static void WriteTempData(QuickData data, string package)
        {
            if (string.IsNullOrEmpty(package))
                package = "global";

            if (!Directory.Exists(QuickKeep.DataPath))
                Directory.CreateDirectory(QuickKeep.DataPath);

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
                if (File.Exists(QuickKeep.GetDirectory(package)))
                {
                    List<string> file = File.ReadAllLines(QuickKeep.GetDirectory(package)).ToList();

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
    }
}

