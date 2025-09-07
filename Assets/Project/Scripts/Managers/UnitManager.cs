using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o cache de unidades ativas para otimizar performance
/// Elimina a necessidade de FindObjectsOfType frequentes
/// </summary>
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }
    
    // Cache separado para unidades do jogador e inimigos
    private HashSet<UnitController> _playerUnits = new HashSet<UnitController>();
    private HashSet<UnitController> _enemyUnits = new HashSet<UnitController>();
    
    // Listas para retorno (evita criação de arrays a cada chamada)
    private List<UnitController> _playerUnitsList = new List<UnitController>();
    private List<UnitController> _enemyUnitsList = new List<UnitController>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Registra uma unidade no cache quando ela é spawned/ativada
    /// </summary>
    public void RegisterUnit(UnitController unit)
    {
        if (unit == null) return;
        
        if (unit.isPlayer)
        {
            _playerUnits.Add(unit);
        }
        else
        {
            _enemyUnits.Add(unit);
        }
        
        // Debug.Log($"Unidade registrada: {unit.name}, Total Player: {_playerUnits.Count}, Total Enemy: {_enemyUnits.Count}");
    }
    
    /// <summary>
    /// Remove uma unidade do cache quando ela morre/é desativada
    /// </summary>
    public void UnregisterUnit(UnitController unit)
    {
        if (unit == null) return;
        
        if (unit.isPlayer)
        {
            _playerUnits.Remove(unit);
        }
        else
        {
            _enemyUnits.Remove(unit);
        }
        
        // Debug.Log($"Unidade removida: {unit.name}, Total Player: {_playerUnits.Count}, Total Enemy: {_enemyUnits.Count}");
    }
    
    /// <summary>
    /// Retorna todas as unidades inimigas ativas (para unidades do jogador procurarem alvos)
    /// </summary>
    public List<UnitController> GetEnemyUnits()
    {
        _enemyUnitsList.Clear();
        
        // Remove unidades mortas/inativas do cache
        _enemyUnits.RemoveWhere(unit => unit == null || !unit.gameObject.activeInHierarchy || unit.isDead);
        
        _enemyUnitsList.AddRange(_enemyUnits);
        return _enemyUnitsList;
    }
    
    /// <summary>
    /// Retorna todas as unidades do jogador ativas (para inimigos procurarem alvos)
    /// </summary>
    public List<UnitController> GetPlayerUnits()
    {
        _playerUnitsList.Clear();
        
        // Remove unidades mortas/inativas do cache
        _playerUnits.RemoveWhere(unit => unit == null || !unit.gameObject.activeInHierarchy || unit.isDead);
        
        _playerUnitsList.AddRange(_playerUnits);
        return _playerUnitsList;
    }
    
    /// <summary>
    /// Encontra a unidade inimiga mais próxima para uma unidade específica
    /// </summary>
    public UnitController FindClosestEnemyUnit(UnitController searcher)
    {
        if (searcher == null) return null;
        
        var targetUnits = searcher.isPlayer ? GetEnemyUnits() : GetPlayerUnits();
        
        float closestDistance = Mathf.Infinity;
        UnitController closestUnit = null;
        Vector3 searcherPosition = searcher.transform.position;
        
        foreach (var unit in targetUnits)
        {
            if (unit == null || unit.isDead || !unit.gameObject.activeInHierarchy) continue;
            
            float distance = Vector3.Distance(searcherPosition, unit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestUnit = unit;
            }
        }
        
        return closestUnit;
    }
    
    /// <summary>
    /// Limpa todos os caches - útil para reset de jogo
    /// </summary>
    public void ClearAllCaches()
    {
        _playerUnits.Clear();
        _enemyUnits.Clear();
        _playerUnitsList.Clear();
        _enemyUnitsList.Clear();
    }
    
    /// <summary>
    /// Força uma limpeza do cache removendo referências nulas
    /// </summary>
    public void CleanupCache()
    {
        _playerUnits.RemoveWhere(unit => unit == null || !unit.gameObject.activeInHierarchy);
        _enemyUnits.RemoveWhere(unit => unit == null || !unit.gameObject.activeInHierarchy);
    }
    
    // Métodos para debug/monitoring
    public int GetPlayerUnitsCount() => _playerUnits.Count;
    public int GetEnemyUnitsCount() => _enemyUnits.Count;
}
