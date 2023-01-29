using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace Serialization
{
    public class SerializationManager
    {
        public static async Task Save(string directory, string fileName, object saveData)
        {
            string json = JsonUtility.ToJson(saveData);
            
            await Task.Run(() =>
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string path = $"{directory}/{fileName}.txt";
                File.WriteAllText(path, json);
            });
        }

        public static async Task<T> Load<T>(string path)
        {
            if (!File.Exists(path)) return default;
            T saveObject = default;

            await Task.Run(() =>
            {
                saveObject = JsonUtility.FromJson<T>(path);
            });
            return saveObject;
        }
        
    }
}