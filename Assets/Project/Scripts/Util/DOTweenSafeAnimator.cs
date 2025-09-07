using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// Sistema seguro para animações DOTween que previne execuções múltiplas
/// </summary>
public class DOTweenSafeAnimator : MonoBehaviour
{
    private Dictionary<string, Tween> _activeTweens = new Dictionary<string, Tween>();
    private HashSet<string> _animatingKeys = new HashSet<string>();

    /// <summary>
    /// Executa uma animação de escala de forma segura
    /// </summary>
    public Tween SafeScale(Transform target, Vector3 endValue, float duration, string animationKey = "scale")
    {
        return SafeExecute(animationKey, () => target.DOScale(endValue, duration));
    }

    /// <summary>
    /// Executa uma animação de movimento de forma segura
    /// </summary>
    public Tween SafeMove(Transform target, Vector3 endValue, float duration, string animationKey = "move")
    {
        return SafeExecute(animationKey, () => target.DOMove(endValue, duration));
    }

    /// <summary>
    /// Executa uma animação de punch scale de forma segura
    /// </summary>
    public Tween SafePunchScale(Transform target, Vector3 punch, float duration, string animationKey = "punch")
    {
        return SafeExecute(animationKey, () => target.DOPunchScale(punch, duration, 0, 0.01f));
    }

    /// <summary>
    /// Executa uma animação de fade de forma segura
    /// </summary>
    public Tween SafeFade(CanvasGroup target, float endValue, float duration, string animationKey = "fade")
    {
        return SafeExecute(animationKey, () => target.DOFade(endValue, duration));
    }

    /// <summary>
    /// Executa qualquer tween de forma segura
    /// </summary>
    public Tween SafeExecute(string animationKey, System.Func<Tween> tweenCreator)
    {
        // Se já está animando esta chave, retorna null
        if (_animatingKeys.Contains(animationKey))
        {
            return null;
        }

        // Mata qualquer tween ativo com esta chave
        KillTween(animationKey);

        // Marca como animando
        _animatingKeys.Add(animationKey);

        // Cria e configura o novo tween
        Tween newTween = tweenCreator();
        if (newTween != null)
        {
            _activeTweens[animationKey] = newTween;
            
            // Remove da lista quando completar
            newTween.OnComplete(() =>
            {
                _animatingKeys.Remove(animationKey);
                _activeTweens.Remove(animationKey);
            });

            // Remove da lista se for morto manualmente
            newTween.OnKill(() =>
            {
                _animatingKeys.Remove(animationKey);
                _activeTweens.Remove(animationKey);
            });
        }
        else
        {
            _animatingKeys.Remove(animationKey);
        }

        return newTween;
    }

    /// <summary>
    /// Mata um tween específico
    /// </summary>
    public void KillTween(string animationKey)
    {
        if (_activeTweens.TryGetValue(animationKey, out Tween tween))
        {
            tween?.Kill();
            _activeTweens.Remove(animationKey);
            _animatingKeys.Remove(animationKey);
        }
    }

    /// <summary>
    /// Mata todos os tweens ativos
    /// </summary>
    public void KillAllTweens()
    {
        foreach (var tween in _activeTweens.Values)
        {
            tween?.Kill();
        }
        _activeTweens.Clear();
        _animatingKeys.Clear();
    }

    /// <summary>
    /// Verifica se uma animação específica está rodando
    /// </summary>
    public bool IsAnimating(string animationKey)
    {
        return _animatingKeys.Contains(animationKey);
    }

    /// <summary>
    /// Verifica se há qualquer animação rodando
    /// </summary>
    public bool IsAnyAnimating()
    {
        return _animatingKeys.Count > 0;
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void OnDisable()
    {
        KillAllTweens();
    }
}

/// <summary>
/// Extensão estática para uso mais fácil do DOTweenSafeAnimator
/// </summary>
public static class DOTweenSafeExtensions
{
    /// <summary>
    /// Obtém ou cria um DOTweenSafeAnimator para o GameObject
    /// </summary>
    public static DOTweenSafeAnimator GetSafeAnimator(this GameObject gameObject)
    {
        var animator = gameObject.GetComponent<DOTweenSafeAnimator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<DOTweenSafeAnimator>();
        }
        return animator;
    }

    /// <summary>
    /// Obtém ou cria um DOTweenSafeAnimator para o Component
    /// </summary>
    public static DOTweenSafeAnimator GetSafeAnimator(this Component component)
    {
        return component.gameObject.GetSafeAnimator();
    }
}
