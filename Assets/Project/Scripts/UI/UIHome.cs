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
    private bool _isStartingGame = false;

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
        // Usa SafeButton para proteger contra cliques duplos
        buttonStartGame.MakeSafe(Button_StartGame, 1.0f); // 1 segundo de cooldown para Battle
        buttonUpdatetGame.MakeSafe(Button_Update, 0.5f);
        buttonSettingsGame.MakeSafe(Button_Settings, 0.5f);
        buttonDiscord.MakeSafe(Button_Discord, 0.3f);
    }

    private void Button_StartGame()
    {
        // Proteção adicional contra múltiplas execuções
        if (_isStartingGame) return;
        StartCoroutine(IE_StartGame());
    }
    private void Button_Update() => _uiGameController.OpenUpdate();
    private void Button_Settings() => _uiGameController.OpenSettings();
    private void Button_Discord() => Application.OpenURL("https://discord.gg/BhTSC9mT");
    
    private IEnumerator IE_StartGame() 
    {
        _isStartingGame = true;
        
        AudioManager.PlayButtonSound();

        _unitController.ShowOrHideBottomMenu(true);
        ShowOrHideBottomMenu(false);
        _mainCamera.DOOrthoSize(5f, 0.5f).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(.25f);
        AudioManager.Play(audioStartGame, AudioManager.AudioType.FX);
        yield return new WaitForSeconds(.25f);

        GameManager.Instance.InitGame();
        MeatManager.Instance.GenerateMeat(true);
        
        _isStartingGame = false;
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
