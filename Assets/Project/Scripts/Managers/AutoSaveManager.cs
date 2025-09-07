using UnityEngine;
using System.Collections;

/// <summary>
/// Gerencia auto-save automático para WebGL
/// Salva periodicamente e em eventos importantes
/// </summary>
public class AutoSaveManager : MonoBehaviour
{
    public static AutoSaveManager Instance { get; private set; }
    
    [Header("Auto Save Settings")]
    [SerializeField] private float autoSaveInterval = 15f; // Salva a cada 15 segundos (mais frequente para WebGL)
    [SerializeField] private bool enableAutoSave = true;
    
    private Coroutine autoSaveCoroutine;
    private GameManager gameManager;
    
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
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        
        if (enableAutoSave)
        {
            StartAutoSave();
        }
        
        // Salva quando o jogo perde foco (importante para WebGL)
        RegisterApplicationEvents();
    }
    
    private void StartAutoSave()
    {
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        
        autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
    }
    
    private IEnumerator AutoSaveRoutine()
    {
        while (enableAutoSave)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            
            // Só salva se o jogo não estiver pausado e tiver dados para salvar
            if (gameManager != null && !gameManager.isPause)
            {
                SaveGame("Auto-save");
            }
        }
    }
    
    private void RegisterApplicationEvents()
    {
        // Salva quando o jogador sai da aba/janela
        Application.focusChanged += OnApplicationFocus;
        
        // Salva quando o jogo é pausado (mobile)
      //  Application.pauseChanged += OnApplicationPause;
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame("Focus lost");
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame("Application pause");
        }
    }
    
    /// <summary>
    /// Força um save imediato
    /// </summary>
    public void ForceSave(string reason = "Manual save")
    {
        SaveGame(reason);
    }
    
    private void SaveGame(string reason)
    {
        if (gameManager == null) return;
        
        try
        {
            // Usa o sistema de save do GameManager
            SaveData data = new SaveData
            {
                gold = gameManager.currentGold,
                currentEra = gameManager.currentEra,
                saveSettings = gameManager.saveSettings,
                saveUpgrade = gameManager.saveUpgrade
            };
            
            SaveManager.Save(data);
            Debug.Log($"Auto-save realizado: {reason}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro no auto-save ({reason}): {e.Message}");
        }
    }
    
    /// <summary>
    /// Habilita/desabilita auto-save
    /// </summary>
    public void SetAutoSaveEnabled(bool enabled)
    {
        enableAutoSave = enabled;
        
        if (enabled)
        {
            StartAutoSave();
        }
        else if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
            autoSaveCoroutine = null;
        }
    }
    
    /// <summary>
    /// Configura intervalo de auto-save
    /// </summary>
    public void SetAutoSaveInterval(float interval)
    {
        autoSaveInterval = Mathf.Max(5f, interval); // Mínimo 5 segundos
        
        if (enableAutoSave)
        {
            StartAutoSave(); // Reinicia com novo intervalo
        }
    }
    
    private void OnDestroy()
    {
        // Remove listeners
        Application.focusChanged -= OnApplicationFocus;
      //  Application.pauseChanged -= OnApplicationPause;
        
        // Save final ao destruir
        if (gameManager != null)
        {
            SaveGame("OnDestroy");
        }
    }
    
    private void OnApplicationQuit()
    {
        // Save final ao sair do jogo
        SaveGame("OnApplicationQuit");
    }
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    
    // No WebGL, também monitora eventos do navegador
    private void Update()
    {
        // Detecta se a página está sendo fechada/recarregada
        if (Input.GetKeyDown(KeyCode.F5) || 
            (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)))
        {
            SaveGame("Page refresh detected");
        }
    }
    
    #endif
}
