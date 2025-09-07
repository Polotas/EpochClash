using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class CurrencyDrop : MonoBehaviour
{
    private Tween _moveTween, _scaleTween, _punchTween, _collectMoveTween, _collectScaleTween;
    private UIGame _uiGame;
    private Camera _worldCamera;
    public int dropValor = 5;
    private bool _onWait;
    private bool _isAnimating = false;
    
    [Header("Dynamic Drop System")]
    public int baseDropValue = 5;
    public int bonusDropValue = 0;
    public bool isBonusDrop = false;

    private void Awake()
    {
        _uiGame = FindFirstObjectByType<UIGame>();
        _worldCamera = Camera.main;
    }

    public void Initialize(Vector3 startPos, float duration = 0.7f, float height = 1f)
    {
        // Proteção contra múltiplas execuções
        if (_isAnimating) return;
        _isAnimating = true;
        
        // Mata todos os tweens ativos para evitar conflitos
        KillAllTweens();
        
        // Calcula valor final do drop
        dropValor = baseDropValue + bonusDropValue;
        
        transform.position = startPos;

        float randomX = Random.Range(-3f, 3f);
        float randomZ = Random.Range(-3, 3);
        Vector3 endPos = new Vector3(startPos.x + randomX, startPos.y, startPos.z + randomZ);

        // Animação diferente para drops bonus
        var punch = isBonusDrop ? new Vector3(0.4f, 0.4f, 0.4f) : new Vector3(0.2f, 0.2f, 0.2f);
        _punchTween = transform.DOPunchScale(punch, 0.3f, 0, 0.01f);
        
        // Movimento em parábola (como bolinha)
        _moveTween = DOTween.To(
            () => 0f,
            t =>
            {
                // Movimento linear em XZ
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);

                // Curva parabólica no Y: sobe e depois cai
                float parabola = 4 * height * (t - t * t); // 0→1→0
                pos.y += parabola;

                transform.position = pos;
            },
            1f, duration
        ).SetEase(Ease.Linear);
        
        // Escala maior para drops bonus
        float randomScale = isBonusDrop ? Random.Range(1.2f, 1.4f) : Random.Range(0.9f, 1.1f);
        transform.localScale = Vector3.one * randomScale;
        _onWait = true;
        StartCoroutine(WaitToAdd(2));
    }
    
    private void KillAllTweens()
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
        _punchTween?.Kill();
        _collectMoveTween?.Kill();
        _collectScaleTween?.Kill();
    }
    
    private IEnumerator WaitToAdd(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        if (_onWait)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, _uiGame.earnedCurrencyPosition.position);
            Vector3 worldPos = _worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _worldCamera.nearClipPlane + 5f));
            
            _collectMoveTween = transform.DOMove(worldPos, 0.5f).SetEase(Ease.InOutQuad)
                .OnComplete(() => _isAnimating = false);
            _collectScaleTween = transform.DOScale(Vector3.zero, 0.6f).SetEase(Ease.InOutQuad);
            
            GameManager.Instance.AddGoldEarned(dropValor);
        }
        else
        {
            _isAnimating = false;
        }

        _onWait = false;
    }
}
