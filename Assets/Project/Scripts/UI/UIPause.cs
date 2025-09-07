using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [Header("Components")]
    public Transform bgPause;
    public Button buttonPause;
    public Button buttonContinue;
    public Button buttonQuit;
    public Button buttonQuitContinue;
    public Transform[] currencyFX;

    [Header("FX")]
    public Transform fxPosition;
    public Transform fxEndPosition;
    
    [Header("Animation BG")]
    public Ease easeIn;
    public Ease easeOut;
    public float animationTime = .3f;
    
    private bool isPause;
    private GameManager _gm;
    private Tween _bgTween;
    private bool _isAnimating = false;
    private bool _isQuitting = false;
    
    private void Awake()
    {
        buttonPause.onClick.AddListener(Button_Pause);
        buttonContinue.onClick.AddListener(Button_Continue);
        buttonQuitContinue.onClick.AddListener(Button_Quit);
        buttonQuit.onClick.AddListener(Button_Quit);
    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    private void Button_Pause()
    {
        buttonQuit.enabled = true;
        buttonQuitContinue.enabled = true;
        AudioManager.PlayButtonSound();
        
        if (isPause)
        {
            Button_Continue();
            return;
        }
        
        if (_isAnimating) return;
        _isAnimating = true;
        
        _gm.Pause(true);
        buttonContinue.gameObject.SetActive(true);
        buttonQuit.gameObject.SetActive(true);
        buttonQuitContinue.gameObject.SetActive(false);
        isPause = true;
        
        _bgTween?.Kill();
        _bgTween = bgPause.DOScale(Vector3.one, animationTime)
            .SetEase(easeIn)
            .SetUpdate(true)
            .OnComplete(() => _isAnimating = false);
        Time.timeScale = 0;
    }

    public void EndGame(bool win)
    {
        _bgTween?.Kill();
        _bgTween = bgPause.DOScale(Vector3.one, animationTime)
            .SetEase(easeIn)
            .SetUpdate(true);
        buttonContinue.gameObject.SetActive(false);
        buttonQuit.gameObject.SetActive(false);
        buttonQuitContinue.gameObject.SetActive(true);
        Time.timeScale = 1;
        _isAnimating = false;
    }
    
    private void Button_Continue()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        
        AudioManager.PlayButtonSound();
        isPause = false;
        _gm.Pause(false);
        
        _bgTween?.Kill();
        _bgTween = bgPause.DOScale(Vector3.zero, animationTime)
            .SetEase(easeOut)
            .SetUpdate(true)
            .OnComplete(() => _isAnimating = false);
        Time.timeScale = 1;
    }

    private void Button_Quit()
    {
        if (_isQuitting) return;
        buttonQuit.enabled = false;
        buttonQuitContinue.enabled = false;
        StartCoroutine(CollectEarned());
    }

    private IEnumerator CollectEarned() 
    {
        _isQuitting = true;
        
        AudioManager.PlayButtonSound();
        var currentEarned = GameManager.Instance.currentGoldEarned;
        Time.timeScale = 1;
        _gm.RemoveUnits();
        
        if (currentEarned >= 5)
        {
            foreach (var t in currencyFX)
            {
                Vector2 basePos = fxPosition.position;
                Vector2 randomOffset = Random.insideUnitSphere * 100f; 
                t.transform.position = basePos;
                
                t.transform.DOMove(basePos + randomOffset, 1).SetEase(easeOut).SetUpdate(true);
                t.transform.DOScale(Vector3.one, 0.5f).SetEase(easeOut).SetUpdate(true);
            }

            yield return new WaitForSeconds(1f);
            
            if (currentEarned >= 5)
            {
                foreach (var t in currencyFX)
                {
                    var randomTime = Random.Range(0.8f, 1f);
                    t.transform.DOMove(fxEndPosition.position, randomTime).SetEase(easeOut).SetUpdate(true).onComplete = CallAudioCoin;
                    t.transform.DOScale(Vector3.zero, randomTime).SetEase(easeOut).SetUpdate(true);
                }
            }

            yield return new WaitForSeconds(1f);
            _gm.AddGold(currentEarned);
            _gm.AddGoldEarned(-currentEarned);
        }

        isPause = false;
        _gm.Pause(false);
        _gm.EndGame(true);
        
        _bgTween?.Kill();
        _bgTween = bgPause.DOScale(Vector3.zero, animationTime)
            .SetEase(easeOut)
            .SetUpdate(true);
        
        _isQuitting = false;
    }
    
    private void CallAudioCoin() =>AudioManager.PlayCollectCoin();
}
