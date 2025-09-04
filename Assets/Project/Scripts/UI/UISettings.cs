using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    public Transform bgSettings;
    public Button buttonQuit;
    public Slider sliderAudioBG;
    public Slider sliderAudioFX;
    
    public Ease easeIn;
    public Ease easeOut;
    public float animationTime = .3f;
    private UIGameController _uIGameController;
    private GameManager _gm;
    
    private void Awake()
    {
        _uIGameController = FindFirstObjectByType<UIGameController>();
        buttonQuit.onClick.AddListener(Button_Closed);
        sliderAudioBG.onValueChanged.AddListener(OnChangeValueAudioBG);
        sliderAudioFX.onValueChanged.AddListener(OnChangeValueAudioFX);
    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    public void InitUpdate()
    {
        sliderAudioBG.value = _gm.saveSettings.bgAudio;
        sliderAudioFX.value = _gm.saveSettings.fxAudio;
        AudioManager.SetVolume(AudioManager.AudioType.BG,sliderAudioBG.value);
        AudioManager.SetVolume(AudioManager.AudioType.FX,sliderAudioFX.value);
    }
    
    public void Open()
    {
        AudioManager.PlayButtonSound();
        bgSettings.DOScale(Vector3.one, animationTime).SetEase(easeIn).SetDelay(.5f); 
    }

    public void Close()
    {
        AudioManager.PlayButtonSound();
        bgSettings.DOScale(Vector3.zero, animationTime).SetEase(easeOut);
    }

    private void Button_Closed() => _uIGameController.CloseSetting();

    private void OnChangeValueAudioBG(float valor) => _gm.ChangeAudioBG(valor);
    private void OnChangeValueAudioFX(float valor) => _gm.ChangeAudioFX(valor);
}
