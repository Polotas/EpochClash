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
    private GameManager gm;
    
    private void Awake()
    {
        buttonPause.onClick.AddListener(Button_Pause);
        buttonContinue.onClick.AddListener(Button_Continue);
        buttonQuit.onClick.AddListener(Button_Quit);
        buttonQuitContinue.onClick.AddListener(Button_Quit);
    }

    private void Start()
    {
        gm = GameManager.Instance;
    }

    private void Button_Pause()
    {
        AudioManager.PlayButtonSound();
        
        if (isPause)
        {
            Button_Continue();
            return;
        }
        
        gm.Pause(true);
        buttonContinue.gameObject.SetActive(true);
        buttonQuit.gameObject.SetActive(true);
        buttonQuitContinue.gameObject.SetActive(false);
        isPause = true;
        bgPause.DOScale(Vector3.one, animationTime).SetEase(easeIn).SetUpdate(true);
        Time.timeScale = 0;
    }

    public void EndGame(bool win)
    {
        bgPause.DOScale(Vector3.one, animationTime).SetEase(easeIn).SetUpdate(true);
        buttonContinue.gameObject.SetActive(false);
        buttonQuit.gameObject.SetActive(false);
        buttonQuitContinue.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
    
    private void Button_Continue()
    {
        AudioManager.PlayButtonSound();
        isPause = false;
        gm.Pause(false);
        bgPause.DOScale(Vector3.zero, animationTime).SetEase(easeOut).SetUpdate(true);
        Time.timeScale = 1;
    }

    private void Button_Quit() => StartCoroutine(CollectEarned());

    private IEnumerator CollectEarned() 
    {
        AudioManager.PlayButtonSound();
        var currentEarned = GameManager.Instance.currentGoldEarned;
        Time.timeScale = 1;

        if (currentEarned >= 5)
        {
            foreach (var t in currencyFX)
            {
                // posição base (centro da esfera)
                Vector2 basePos = fxPosition.position;

                // gera posição aleatória dentro de uma esfera
                Vector2 randomOffset = Random.insideUnitSphere * 100f; // raio = 2 (pode ajustar)

                // aplica a posição inicial
                t.transform.position = basePos + randomOffset;

                t.transform.localScale = Vector3.one;
                var randomTime = Random.Range(0.15f, 0.3f);
                t.transform.DOMove(fxEndPosition.position, randomTime).SetEase(easeOut).SetUpdate(true);
                t.transform.DOScale(Vector3.zero, randomTime).SetEase(easeOut).SetUpdate(true);
            }
            
            yield return new WaitForSeconds(.2f);
            GameManager.Instance.AddGold(currentEarned);
            GameManager.Instance.AddGoldEarned(-currentEarned);
            AudioManager.PlayCollectCoin();
        }

        isPause = false;
        gm.Pause(false);
        gm.EndGame(true);
        bgPause.DOScale(Vector3.zero, animationTime).SetEase(easeOut).SetUpdate(true);

    }
}
