using UnityEngine;
using System.Collections.Generic;

public enum GameEra
{
    StoneAge,
    SpartanAge,
    EgyptianAge,
    MedievalAge,
    ModernAge
}

public enum WarriorType
{
    Simple,
    Simple2,
    Big
}

public class EraManager : MonoBehaviour
{
    public static EraManager Instance { get; private set; }

    public GameEra currentEra = GameEra.StoneAge;
    public EnemyEraManager enemyEraManager;
    public List<GameObject> eraSpecificAssets;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateEraAssets();
    }

    public void AdvanceEra()
    {
        if (currentEra < GameEra.ModernAge)
        {
            currentEra++;
            Debug.Log("Advanced to " + currentEra.ToString() + "!");
            UpdateEraAssets();
        }
        else
        {
            Debug.Log("Already in the Modern Age, no more eras to advance.");
        }
    }

    private void UpdateEraAssets()
    {
        // Logic to activate/deactivate assets based on currentEra
        // For example, enable specific unit prefabs, change background, etc.
        Debug.Log("Updating assets for " + currentEra.ToString() + "...");
        // This would typically involve more complex asset loading/unloading
    }
}