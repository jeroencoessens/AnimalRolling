using UnityEngine;
using System.IO;

/// <summary>
/// Handles save/load of persistent data using JSON serialization.
/// </summary>
public static class SaveManager
{
    private const string SAVE_FILE = "animal_escape_save.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE);

    public static PersistentData Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<PersistentData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to load save: " + e.Message);
            }
        }
        return new PersistentData();
    }

    public static void Save(PersistentData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
