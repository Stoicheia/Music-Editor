﻿using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace Serialization
{
    public static class SerializationManager
    {
        public static async Task Save(string path, object saveData)
        {
            string json = JsonUtility.ToJson(saveData);
            
            await Task.Run(() =>
            {
                File.WriteAllText(path, json);
            });
        }

        public static async Task<T> Load<T>(string path)
        {
            if (!File.Exists(path)) return default;
            T saveObject = default;

            await Task.Run(() =>
            {
                try
                {
                    string fileContents = File.ReadAllText(path);
                    saveObject = JsonUtility.FromJson<T>(fileContents);
                }
                catch
                {
                    throw new FileLoadException($"Could not deserialize JSON file at {path}.");
                }
            });

            return saveObject;
        }
        
    }
}