using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class CurrencyDrop : MonoBehaviour
{
    private Tween _moveTween, _scaleTween;
    private UIGame _uiGame;
    private Camera _worldCamera;
    public int dropValor = 5;
    private bool _onWait;

    private void Awake()
    {
        _uiGame = FindFirstObjectByType<UIGame>();
        _worldCamera = Camera.main;
    }

    public void Initialize(Vector3 startPos, float duration = 0.7f, float height = 1f)
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();
        
        transform.position = startPos;

        float randomX = Random.Range(-3f, 3f);
        float randomZ = Random.Range(-3, 3);
        Vector3 endPos = new Vector3(startPos.x + randomX, startPos.y, startPos.z + randomZ);

        var punch = new Vector3(0.2f, 0.2f, 0.2f);
        transform.DOPunchScale(punch, 0.3f,0,0.01f);
        
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
        
        float randomScale = Random.Range(0.9f, 1.1f);
        transform.localScale = Vector3.one * randomScale;
        _onWait = true;
        StartCoroutine(WaitToAdd(2));
    }
    
    private IEnumerator WaitToAdd(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        if (_onWait)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, _uiGame.earnedCurrencyPosition.position);
            Vector3 worldPos = _worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _worldCamera.nearClipPlane + 5f));
            transform.DOMove(worldPos, 0.5f).SetEase(Ease.InOutQuad);
            transform.DOScale(Vector3.zero, 0.6f).SetEase(Ease.InOutQuad);
            GameManager.Instance.AddGoldEarned(dropValor);
        }

        _onWait = false;
    }
}
