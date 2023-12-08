using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class SaveLoadSystem
{
    public static int RecentlyVersion { get; } = 4; // Version Change?

    public static string SaveDirectory
    {
        get
        {
            return $"{Application.persistentDataPath}/Save";
        }
    }

    public static void Save(SaveData data, string fileName)
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        var path = Path.Combine(SaveDirectory, fileName);
        using (var writer = new JsonTextWriter(new StreamWriter(path))) // Add Converter?
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new Vector3Converter());
            serializer.Converters.Add(new QuaternionConverter());

            serializer.Serialize(writer, data);
        }
    }

    public static SaveData Load(string fileName)
    {
        var path = Path.Combine(SaveDirectory, fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        SaveData result = null;
        int version = 0;

        var json = File.ReadAllText(path);
        using (var reader = new JsonTextReader(new StringReader(json)))
        {
            var jObj = JObject.Load(reader);
            version = jObj["Version"].Value<int>();
        }
        using (var reader = new JsonTextReader(new StringReader(json)))
        {
            var serializer = new JsonSerializer(); // Add Converter?
            serializer.Converters.Add(new Vector3Converter());
            serializer.Converters.Add(new QuaternionConverter());

            switch (version) // Add Version?
            {
                case 1:
                    result = serializer.Deserialize<SaveDataV1>(reader);
                    break;
                case 2:
                    result = serializer.Deserialize<SaveDataV2>(reader);
                    break;
                case 3:
                    result = serializer.Deserialize<SaveDataV3>(reader);
                    break;
                case 4:
                    result = serializer.Deserialize<SaveDataV4>(reader);
                    break;
                    
            }
            while (result.Version < RecentlyVersion)
            {
                result = result.VersionUp();
            }
        }
        return result;
    }

    public static void Remove(string fileName)
    {
        var path = Path.Combine(SaveDirectory, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}