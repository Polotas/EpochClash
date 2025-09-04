using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    public RectTransform  earnedCurrencyPosition;
    public CanvasGroup canvasGroupCurrentRound;
    public TextMeshProUGUI textCurrentRound;
    private Coroutine _coroutineCurrentRound;
    private EnemySpawner _enemySpawner;
    
    private void Start()
    {
        _enemySpawner = FindFirstObjectByType<EnemySpawner>();
        _enemySpawner.onUpdateCurrentRound += UpdateRound;
        GameManager.Instance.onCurrencyEarnedChanged += CheckActiveEarnedCurrency;
        Invoke("DisableEarned",.1f);
    }

    private void DisableEarned() => CheckActiveEarnedCurrency(0);
    
    private void CheckActiveEarnedCurrency(int valor) => earnedCurrencyPosition.gameObject.SetActive(valor != 0);

    public void UpdateRound(int currentRound) => _coroutineCurrentRound = StartCoroutine(IE_UpdateRound(currentRound));
    
    private IEnumerator IE_UpdateRound(int currentRound)
    {
        textCurrentRound.text = "Round " + currentRound;
        canvasGroupCurrentRound.DOFade(1, 0.5f);
        yield return new WaitForSeconds(5);
        canvasGroupCurrentRound.DOFade(0, 0.5f);
    }

    private void EndGame()
    {
        StopCoroutine(_coroutineCurrentRound);
        canvasGroupCurrentRound.DOFade(0, 0.1f);
    }

}
