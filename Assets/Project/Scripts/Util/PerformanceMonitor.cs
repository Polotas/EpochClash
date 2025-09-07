using UnityEngine;
using System.Diagnostics;

/// <summary>
/// Monitor de performance para verificar melhorias no sistema de cache de unidades
/// Adicione este componente a um GameObject na cena para ver as estatísticas
/// </summary>
public class PerformanceMonitor : MonoBehaviour
{
    [Header("Configurações")]
    public bool showDebugInfo = true;
    public float updateInterval = 1f;
    
    [Header("Estatísticas (Read Only)")]
    [SerializeField] private int totalPlayerUnits = 0;
    [SerializeField] private int totalEnemyUnits = 0;
    [SerializeField] private float avgFindTargetTime = 0f;
    [SerializeField] private int findTargetCalls = 0;
    
    private float timer = 0f;
    private Stopwatch stopwatch = new Stopwatch();
    private float totalFindTargetTime = 0f;
    
    // GUI
    private Rect windowRect = new Rect(10, 10, 300, 200);
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= updateInterval)
        {
            UpdateStatistics();
            timer = 0f;
        }
    }
    
    private void UpdateStatistics()
    {
        if (UnitManager.Instance != null)
        {
            totalPlayerUnits = UnitManager.Instance.GetPlayerUnitsCount();
            totalEnemyUnits = UnitManager.Instance.GetEnemyUnitsCount();
        }
        
        if (findTargetCalls > 0)
        {
            avgFindTargetTime = (totalFindTargetTime / findTargetCalls) * 1000f; // em milissegundos
        }
    }
    
    /// <summary>
    /// Método para medir o tempo de execução do FindNewTarget
    /// Chame este método antes e depois da operação
    /// </summary>
    public void StartFindTargetTimer()
    {
        stopwatch.Restart();
    }
    
    public void EndFindTargetTimer()
    {
        stopwatch.Stop();
        totalFindTargetTime += (float)stopwatch.Elapsed.TotalSeconds;
        findTargetCalls++;
    }
    
    public void ResetStatistics()
    {
        totalFindTargetTime = 0f;
        findTargetCalls = 0;
        avgFindTargetTime = 0f;
    }
    
    private void OnGUI()
    {
        if (!showDebugInfo) return;
        
        windowRect = GUI.Window(0, windowRect, DrawWindow, "Performance Monitor");
    }
    
    private void DrawWindow(int windowID)
    {
        GUILayout.BeginVertical();
        
        GUILayout.Label("=== Cache de Unidades ===");
        GUILayout.Label($"Unidades do Jogador: {totalPlayerUnits}");
        GUILayout.Label($"Unidades Inimigas: {totalEnemyUnits}");
        GUILayout.Label($"Total de Unidades: {totalPlayerUnits + totalEnemyUnits}");
        
        GUILayout.Space(10);
        GUILayout.Label("=== Performance FindTarget ===");
        GUILayout.Label($"Tempo Médio: {avgFindTargetTime:F2}ms");
        GUILayout.Label($"Total de Chamadas: {findTargetCalls}");
        
        GUILayout.Space(10);
        if (GUILayout.Button("Reset Estatísticas"))
        {
            ResetStatistics();
        }
        
        if (GUILayout.Button("Force Cleanup Cache"))
        {
            if (UnitManager.Instance != null)
            {
                UnitManager.Instance.CleanupCache();
            }
        }
        
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
    
    private void OnEnable()
    {
        if (showDebugInfo)
        {
            UnityEngine.Debug.Log("PerformanceMonitor ativado. Monitorando cache de unidades...");
        }
    }
}
