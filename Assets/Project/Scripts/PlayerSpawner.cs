using System.Collections.Generic;
using UnityEngine;

public enum Player_Spawner_Type
{
    MELEE,
    RANGE,
    BIG
}

public class PlayerSpawner : MonoBehaviour
{
    private EnemySpawner _enemySpawner;
    public Base baseSpawner;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform hitPoint;

    [Header("Prefabs de Unidades do Jogador")]
    public UnitController melee;
    public UnitController range;
    public UnitController big;

    [Header("Pool Settings")]
    public int initialPoolSize = 5; // quantas instâncias criar de cada tipo

    private Dictionary<Player_Spawner_Type, Queue<UnitController>> poolDict;
    
    [Header("Ativos em Cena")]
    public List<UnitController> currentSpawn;

    private GameManager _gm;
    
    private void Awake()
    {
        _enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    void Start()
    {
        currentSpawn = new List<UnitController>();
        poolDict = new Dictionary<Player_Spawner_Type, Queue<UnitController>>();
        _gm = GameManager.Instance;
        _gm.onInitGame += InitGame;
        
        
        // cria pool inicial para cada tipo de unidade
        CreatePool(Player_Spawner_Type.MELEE, melee);
        CreatePool(Player_Spawner_Type.RANGE, range);
        CreatePool(Player_Spawner_Type.BIG, big);
    }

    private void InitGame()
    {
        baseSpawner.health.health = _gm.saveUpgrade.baseLive.getCurrentStatsMultiplier();
        baseSpawner.health.Reset();
    }
    
    private void CreatePool(Player_Spawner_Type type, UnitController prefab)
    {
        Queue<UnitController> pool = new Queue<UnitController>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.gameObject.SetActive(false);
            obj.enemySpawner = _enemySpawner;
            pool.Enqueue(obj);
        }
        poolDict[type] = pool;
    }

    public void SpawnUnit(Player_Spawner_Type type)
    {
        if (!poolDict.ContainsKey(type)) return;

        var position = spawnPoint.position;
        position.z = Random.Range(-.5f, .5f);
        
        UnitController obj;
        if (poolDict[type].Count > 0)
        {
            obj = poolDict[type].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
            obj.gameObject.SetActive(true);
        }
        else
        {
            // se a pool acabar, instancia novo
            obj = Instantiate(GetPrefab(type), spawnPoint.position, Quaternion.identity);
            obj.enemySpawner = _enemySpawner;
        }

        _enemySpawner.ReFindUnits();
        currentSpawn.Add(obj);
    }

    private UnitController GetPrefab(Player_Spawner_Type type)
    {
        return type switch
        {
            Player_Spawner_Type.MELEE => melee,
            Player_Spawner_Type.RANGE => range,
            Player_Spawner_Type.BIG => big,
            _ => melee
        };
    }
    
    public void ReFindUnits()
    {
        foreach (var t in currentSpawn)
        {
            t.FindNewTarget();
        }
    }
    
    public void EndGame()
    {
        foreach (var t in currentSpawn)
        {
            t.gameObject.SetActive(false);
        }
        
        baseSpawner.health.Reset();
    }
}
