using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
    public AudioClip audioStartGame;
    public Button buttonStartGame;
    public Button buttonUpdatetGame;
    public Button buttonSettingsGame;
    public Button buttonDiscord;
    public Transform bottomMenu;
    public Transform topMenu;
    public Transform logo;
    private UIUnitController _unitController;
    private UIGameController _uiGameController;
    private EnemySpawner _enemySpawner;
    private Camera _mainCamera;

    [Header("Animation BG")]
    public float bottomShow;
    public float bottomHide;
    public float topShow;
    public float topHide;
    
    private void Awake()
    {
        _uiGameController = FindFirstObjectByType<UIGameController>();
        _unitController = FindFirstObjectByType<UIUnitController>();
        _enemySpawner = FindFirstObjectByType<EnemySpawner>();
        _mainCamera = Camera.main;
        buttonStartGame.onClick.AddListener(Button_StartGame);
        buttonUpdatetGame.onClick.AddListener(Button_Update);
        buttonSettingsGame.onClick.AddListener(Button_Settings);
        buttonDiscord.onClick.AddListener(Button_Discord);
    }

    private void Button_StartGame() => StartCoroutine(IE_StartGame());
    private void Button_Update() => _uiGameController.OpenUpdate();
    private void Button_Settings() => _uiGameController.OpenSettings();
    private void Button_Discord() => Application.OpenURL("https://discord.gg/BhTSC9mT");
    
    private IEnumerator IE_StartGame() 
    {
        AudioManager.PlayButtonSound();

        _unitController.ShowOrHideBottomMenu(true);
        ShowOrHideBottomMenu(false);
        _mainCamera.DOOrthoSize(5f, 0.5f).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(.25f);
        AudioManager.Play(audioStartGame, AudioManager.AudioType.FX);
        yield return new WaitForSeconds(.25f);

        GameManager.Instance.InitGame();
        MeatManager.Instance.GenerateMeat(true);
    }

    public void EndGame()
    {
        _unitController.ShowOrHideBottomMenu(false);
        ShowOrHideBottomMenu(true);
        _mainCamera.DOOrthoSize(6f, 0.5f).SetEase(Ease.InOutSine);
    }
    
    public void ShowOrHideBottomMenu(bool show)
    {
        if (show)
        {
            bottomMenu.DOLocalMoveY(bottomShow, 0.5f).SetEase(Ease.InBack);
            topMenu.DOLocalMoveY(topShow, 0.5f).SetEase(Ease.InBack);
            logo.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuint).SetDelay(.25f);
        }
        else
        {
            bottomMenu.DOLocalMoveY(bottomHide, 0.5f).SetEase(Ease.InOutBack);
            topMenu.DOLocalMoveY(topHide, 0.5f).SetEase(Ease.InOutBack);
            logo.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuint);
        }
    }
}
