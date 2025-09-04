using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string saveFile => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFile, json);
        Debug.Log($"Save salvo em: {saveFile}");
    }

    public static SaveData Load()
    {
        if (File.Exists(saveFile))
        {
            string json = File.ReadAllText(saveFile);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return null; // ainda não existe save
    }
}