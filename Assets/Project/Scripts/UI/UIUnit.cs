using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnit : MonoBehaviour
{
    public Player_Spawner_Type playerSpawnerType;
    public GameObject objBlock;
    public TextMeshProUGUI textPrice;
    public Image visual;
    public int price;
    
    private Button _button;
    private PlayerSpawner _playerSpawner;
    private Tween _punchTween;
    private bool _isAnimating = false;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _playerSpawner = FindFirstObjectByType<PlayerSpawner>();
        _button.onClick.AddListener(Button_BuyUnit);
    }

    private void Start()
    {
        MeatManager.Instance.OnMeatChanged += UpdateVisual;
    }
    

    public void Setup()
    {
        textPrice.text = price.ToString();
    }

    private void UpdateVisual(int valor)
    {
        if (valor < price)
        {
            objBlock.SetActive(true);
            _button.enabled = false;
        }
        else
        {
            var punch = new Vector3(0.2f, 0.2f, 0.2f);
            if(!_button.enabled && !_isAnimating)
            {
                _punchTween?.Kill();
                _punchTween = transform.DOPunchScale(punch, 0.3f,0,0.01f);
            }
            objBlock.SetActive(false);
            _button.enabled = true;
        }
    }
    
    private void Button_BuyUnit()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        
        AudioManager.PlayButtonSound();
        _playerSpawner.SpawnUnit(playerSpawnerType);
        MeatManager.Instance.AddMeat(-price);
        
        var punch = new Vector3(0.1f, 0.1f, 0.1f);
        _punchTween?.Kill();
        _punchTween = transform.DOPunchScale(punch, 0.3f,0,0.01f)
            .OnComplete(() => _isAnimating = false);
    }
}
