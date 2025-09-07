using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdate : MonoBehaviour
{
    [Header("Components")]
    public Transform bgUpdate;
    public Button buttonQuit;
    
    [Header("Animation BG")]
    public Ease easeIn;
    public Ease easeOut;
    public float animationTime = .3f;

    [Header("Items Update")] 
    [Header("Items Update/Unit 1")]
    public Button buttonUpgradeUnit1;
    public TextMeshProUGUI textPriceUpgradeUnit1;
    public GameObject blockObjectUnit1;
    
    [Header("Items Update/Unit 2")]
    public Button buttonUpgradeUnit2;
    public TextMeshProUGUI textPriceUpgradeUnit2;
    public GameObject blockObjectUnit2;
    
    [Header("Items Update/Unit 3")]
    public Button buttonUpgradeUnit3;
    public TextMeshProUGUI textPriceUpgradeUnit3;
    public GameObject blockObjectUnit3;
    
    [Header("Items Update/Base")]
    public Button buttonUpgradeBase;
    public TextMeshProUGUI textPriceUpgradeBase;
    public TextMeshProUGUI textInfoBase;
    public TextMeshProUGUI textAddLifeBase;
    
    [Header("Items Update/Meat")]
    public Button buttonUpgradeMeat;
    public TextMeshProUGUI textPriceUpgradeMeat;
    public TextMeshProUGUI textInfoMeat;
    
    private UIGameController _uIGameController;
    private GameManager _gm;
    private Tween _bgTween;
    private bool _isAnimating = false;
    
    private void Awake()
    {
        _uIGameController = FindFirstObjectByType<UIGameController>();
        buttonQuit.onClick.AddListener(Button_Closed);
        buttonUpgradeUnit1.onClick.AddListener(Button_UpdateUnit1);
        buttonUpgradeUnit2.onClick.AddListener(Button_UpdateUnit2);
        buttonUpgradeUnit3.onClick.AddListener(Button_UpdateUnit3);
        buttonUpgradeBase.onClick.AddListener(Button_UpdateBase);
        buttonUpgradeMeat.onClick.AddListener(Button_UpdateMeat);
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        _gm.onCurrencyChanged += UpdateUpgradeUI;
    }

    public void Open()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        
        UpdateUpgradeUI();
        AudioManager.PlayButtonSound();
        
        _bgTween?.Kill();
        _bgTween = bgUpdate.DOScale(Vector3.one, animationTime)
            .SetEase(easeIn)
            .SetDelay(.5f)
            .OnComplete(() => _isAnimating = false);
    }

    public void Close()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        
        AudioManager.PlayButtonSound();
        
        _bgTween?.Kill();
        _bgTween = bgUpdate.DOScale(Vector3.zero, animationTime)
            .SetEase(easeOut)
            .OnComplete(() => _isAnimating = false);
    }

    private void UpdateUpgradeUI(int valor = 0)
    {
        var priceBase = _gm.saveUpgrade.baseLive.getPrice();
        var statsBase = _gm.saveUpgrade.baseLive.getCurrentStatsMultiplier();
        var statsBaseAdd = _gm.saveUpgrade.baseLive.getNextAddMultiplier();
        buttonUpgradeBase.interactable = _gm.CheckButtonUnlock(priceBase);
        textPriceUpgradeBase.text = "BUY: " + CurrencyFormatter.FormatCurrency(priceBase);
        textInfoBase.text = "Live: " + CurrencyFormatter.FormatCurrency(statsBase);
        textAddLifeBase.text = "+ " + CurrencyFormatter.FormatCurrency(statsBaseAdd);
        
        var priceMeat = _gm.saveUpgrade.meatSpeed.getPrice();
        var statsMeat = _gm.saveUpgrade.meatSpeed.getCurrentStatsRemove();

        buttonUpgradeMeat.interactable = !_gm.saveUpgrade.meatSpeed.isMaxLevel() && _gm.CheckButtonUnlock(priceMeat);
        
        textPriceUpgradeMeat.text = "BUY: " + CurrencyFormatter.FormatCurrency(priceMeat);
        textInfoMeat.text = "Every: " + CurrencyFormatter.FormatCurrency(statsMeat);
        
        blockObjectUnit2.SetActive(_gm.saveUpgrade.unlockUnit2);
        blockObjectUnit3.SetActive(_gm.saveUpgrade.unlockUnit3);
        buttonUpgradeUnit2.interactable = _gm.CheckButtonUnlock(100); // Reduzido de 150 para 100
        buttonUpgradeUnit3.interactable = _gm.CheckButtonUnlock(250); // Reduzido de 370 para 250
    }
    
    private void Button_Closed() => _uIGameController.CloseUpdate();
    private void Button_UpdateUnit1() => _uIGameController.CloseUpdate();

    private void Button_UpdateUnit2()
    {
        buttonUpgradeUnit2.interactable = false;
        AudioManager.PlayButtonSound();
        _gm.UnlockUnit2();
        _gm.AddGold(-100); // Reduzido de -150 para -100
        blockObjectUnit2.SetActive(true);
    } 
    private void Button_UpdateUnit3()    
    {
        buttonUpgradeUnit3.interactable = false;
        AudioManager.PlayButtonSound();
        _gm.UnlockUnit3();
        _gm.AddGold(-250); // Reduzido de -370 para -250
        blockObjectUnit3.SetActive(true);
    } 

    private void Button_UpdateBase()
    {
        buttonUpgradeBase.interactable = false;
        AudioManager.PlayButtonSound();
        _gm.UpgradeLiveBase();
    } 

    private void Button_UpdateMeat()
    {
        buttonUpgradeMeat.interactable = false;
        AudioManager.PlayButtonSound();
        _gm.UpgradeMeat();
    } 
}
