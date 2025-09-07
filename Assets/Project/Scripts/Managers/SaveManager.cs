using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string saveFile => Path.Combine(Application.persistentDataPath, "save.json");
    private static string saveKey = "EpochClashSave"; // Chave para PlayerPrefs

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        
        // Salva usando PlayerPrefs (funciona em todas as plataformas)
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save(); // Força save imediato
        
        // Backup adicional em arquivo (quando possível)
        #if !UNITY_WEBGL || UNITY_EDITOR
        try
        {
            File.WriteAllText(saveFile, json);
            Debug.Log($"Save salvo em arquivo: {saveFile}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Não foi possível salvar em arquivo: {e.Message}");
        }
        #endif
        
        Debug.Log("Save realizado com sucesso");
    }

    public static SaveData Load()
    {
        SaveData data = null;
        string json = null;
        
        // Primeiro tenta carregar do PlayerPrefs (funciona em todas as plataformas)
        if (PlayerPrefs.HasKey(saveKey))
        {
            json = PlayerPrefs.GetString(saveKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    data = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log("Save carregado do PlayerPrefs");
                    return data;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Erro ao carregar save do PlayerPrefs: {e.Message}");
                }
            }
        }
        
        // Fallback: tenta carregar do arquivo (quando possível)
        #if !UNITY_WEBGL || UNITY_EDITOR
        try
        {
            if (File.Exists(saveFile))
            {
                json = File.ReadAllText(saveFile);
                data = JsonUtility.FromJson<SaveData>(json);
                
                // Se conseguiu carregar do arquivo, salva no PlayerPrefs também
                if (data != null)
                {
                    PlayerPrefs.SetString(saveKey, json);
                    PlayerPrefs.Save();
                    Debug.Log("Save migrado do arquivo para PlayerPrefs");
                }
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Erro ao carregar save do arquivo: {e.Message}");
        }
        #endif
        
        return null; // Nenhum save encontrado
    }
    
    public static void DeleteSave()
    {
        // Remove do PlayerPrefs
        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.Save();
        
        // Remove arquivo se existir
        #if !UNITY_WEBGL || UNITY_EDITOR
        try
        {
            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Erro ao deletar arquivo de save: {e.Message}");
        }
        #endif
        
        Debug.Log("Save deletado");
    }
    
    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(saveKey) 
        #if !UNITY_WEBGL || UNITY_EDITOR
               || File.Exists(saveFile)
        #endif
               ;
    }
}