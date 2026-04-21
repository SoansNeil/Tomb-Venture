using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int health;
    public int coins;
    public float elapsedTime;
    public string sceneName;
    public List<string> collectedCoinIds = new List<string>();
}

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveSystem] Saved to {SavePath}");
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveSystem] No save file found.");
            return null;
        }
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave() => File.Exists(SavePath);
}
