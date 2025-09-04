using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Base baseSpawner;
    private PlayerSpawner _playerSpawner;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform hitPoint;
    
    [Header("Prefabs de Unidades")]
    public List<UnitController> unitPrefabs; 
    
    [Header("Pool Settings")]
    public int initialPoolSize = 5;
    private Dictionary<UnitController, Queue<UnitController>> poolDict;
    
    public List<UnitController> currentSpawn;
    private GameManager _gm;
    private EraManager _eraManager;
    private Coroutine _spawnCoroutine;

    public Action<int> onUpdateCurrentRound;
    
    private void Awake()
    {
        _playerSpawner = FindFirstObjectByType<PlayerSpawner>();
    }

    private void Start()
    {
        currentSpawn = new List<UnitController>();
        poolDict = new Dictionary<UnitController, Queue<UnitController>>();
        _gm = GameManager.Instance;
        _eraManager = EraManager.Instance;
        _gm.onInitGame += StartEra;
        
        foreach (var prefab in unitPrefabs)
        {
            Queue<UnitController> pool = new Queue<UnitController>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                var obj = Instantiate(prefab, transform);
                obj.gameObject.SetActive(false);
                obj.playerSpawner = _playerSpawner;
                pool.Enqueue(obj);
            }
            poolDict[prefab] = pool;
        }
    }
    
    private void StartEra() => _spawnCoroutine = StartCoroutine(IE_StartEra());

    private IEnumerator IE_StartEra()
    {
        var era = _eraManager.enemyEraManager.eras[0];
        var eraAge = era.eraAges[0];
        var currentRound = 1;
        
        yield return new WaitForSeconds(3);
        
        foreach (var t1 in eraAge.ageRounds)
        {
            onUpdateCurrentRound?.Invoke(currentRound);
            currentRound++;
            
            foreach (var t in t1.roundsWaves)
            {
                for (int j = 0; j < t.quantity; j++)
                {
                    SpawnUnit(unitPrefabs[(int)t.warriorType]); 
                }
                
                yield return new WaitForSeconds(t.delayBetweenRounds);
            }

            yield return new WaitForSeconds(t1.delayBetweenWaves);
        }
        
        yield return new WaitForSeconds(.1f);
    }
    
    private void SpawnUnit(UnitController prefab)
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogError("Prefab ou ponto de spawn não definido!");
            return;
        }
        
        var position = spawnPoint.position;
        position.z = Random.Range(-0.5f, 0.5f);
        
        UnitController obj;
        if (poolDict[prefab].Count > 0)
        {
            obj = poolDict[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            obj.playerSpawner = _playerSpawner;
        }

        _playerSpawner.ReFindUnits();
        currentSpawn.Add(obj);
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
        StopCoroutine(_spawnCoroutine);
        
        foreach (var t in currentSpawn)
        {
            t.gameObject.SetActive(false);
        }
        
        baseSpawner.health.Reset();
    }
}