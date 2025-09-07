using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Sistema de proteção contra cliques duplos/múltiplos para botões
/// Previne execução múltipla de funções em WebGL e outras plataformas
/// </summary>
public class SafeButton : MonoBehaviour
{
    [Header("Configurações de Proteção")]
    [SerializeField] private float cooldownTime = 0.5f; // Tempo de cooldown entre cliques
    [SerializeField] private bool disableVisually = true; // Se deve desabilitar visualmente o botão
    [SerializeField] private bool debugLogs = false; // Logs para debug
    
    private Button _button;
    private bool _isOnCooldown = false;
    private bool _originalInteractable;
    private System.Action _originalCallback;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError($"SafeButton: Não foi encontrado componente Button em {gameObject.name}");
            return;
        }
        
        _originalInteractable = _button.interactable;
        
        // Remove todos os listeners existentes e adiciona nossa proteção
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnSafeClick);
    }
    
    /// <summary>
    /// Define o callback que será executado de forma segura
    /// </summary>
    public void SetCallback(System.Action callback)
    {
        _originalCallback = callback;
    }
    
    /// <summary>
    /// Adiciona um callback de forma segura (mantém callbacks existentes)
    /// </summary>
    public void AddCallback(System.Action callback)
    {
        var existingCallback = _originalCallback;
        _originalCallback = () =>
        {
            existingCallback?.Invoke();
            callback?.Invoke();
        };
    }
    
    private void OnSafeClick()
    {
        // Se está em cooldown, ignora o clique
        if (_isOnCooldown)
        {
            if (debugLogs)
                Debug.Log($"SafeButton: Clique ignorado em {gameObject.name} (cooldown ativo)");
            return;
        }
        
        // Inicia cooldown
        StartCoroutine(CooldownRoutine());
        
        // Executa callback original
        if (_originalCallback != null)
        {
            if (debugLogs)
                Debug.Log($"SafeButton: Executando callback em {gameObject.name}");
            
            try
            {
                _originalCallback.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SafeButton: Erro ao executar callback em {gameObject.name}: {e.Message}");
            }
        }
        else if (debugLogs)
        {
            Debug.LogWarning($"SafeButton: Nenhum callback definido para {gameObject.name}");
        }
    }
    
    private IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;
        
        // Desabilita visualmente se configurado
        if (disableVisually && _button != null)
        {
            _button.interactable = false;
        }
        
        if (debugLogs)
            Debug.Log($"SafeButton: Iniciando cooldown de {cooldownTime}s em {gameObject.name}");
        
        yield return new WaitForSecondsRealtime(cooldownTime);
        
        _isOnCooldown = false;
        
        // Reabilita o botão se estava desabilitado visualmente
        if (disableVisually && _button != null)
        {
            _button.interactable = _originalInteractable;
        }
        
        if (debugLogs)
            Debug.Log($"SafeButton: Cooldown finalizado em {gameObject.name}");
    }
    
    /// <summary>
    /// Força o fim do cooldown (use com cuidado)
    /// </summary>
    public void ForceEndCooldown()
    {
        StopAllCoroutines();
        _isOnCooldown = false;
        if (_button != null)
        {
            _button.interactable = _originalInteractable;
        }
    }
    
    /// <summary>
    /// Verifica se o botão está em cooldown
    /// </summary>
    public bool IsOnCooldown()
    {
        return _isOnCooldown;
    }
    
    /// <summary>
    /// Configura o tempo de cooldown dinamicamente
    /// </summary>
    public void SetCooldownTime(float newCooldownTime)
    {
        cooldownTime = Mathf.Max(0.1f, newCooldownTime);
    }
    
    /// <summary>
    /// Habilita/desabilita logs de debug
    /// </summary>
    public void SetDebugLogs(bool enabled)
    {
        debugLogs = enabled;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

/// <summary>
/// Extensões para facilitar o uso do SafeButton
/// </summary>
public static class SafeButtonExtensions
{
    /// <summary>
    /// Converte um botão normal em SafeButton
    /// </summary>
    public static SafeButton MakeSafe(this Button button, System.Action callback, float cooldownTime = 0.5f)
    {
        var safeButton = button.GetComponent<SafeButton>();
        if (safeButton == null)
        {
            safeButton = button.gameObject.AddComponent<SafeButton>();
        }
        
        safeButton.SetCallback(callback);
        safeButton.SetCooldownTime(cooldownTime);
        
        return safeButton;
    }
    
    /// <summary>
    /// Adiciona proteção rápida a um botão existente
    /// </summary>
    public static void AddSafeClick(this Button button, System.Action callback, float cooldownTime = 0.5f)
    {
        button.MakeSafe(callback, cooldownTime);
    }
}
