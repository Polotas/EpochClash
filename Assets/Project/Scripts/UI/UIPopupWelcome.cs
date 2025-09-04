using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupWelcome : MonoBehaviour
{
    [Header("Components")]
    public Transform bg;
    public Transform bgBlack;
    public Button buttonQuit;
    
    [Header("Animation BG")]
    public Ease easeIn;
    public Ease easeOut;
    public float animationTime = .3f;
    
    private void Awake()
    {
        buttonQuit.onClick.AddListener(Close);
    }
    
    public void Open()
    {
        bgBlack.gameObject.SetActive(true);
        bg.DOScale(Vector3.one, animationTime).SetEase(easeIn).SetDelay(.5f); 
    }

    private void Close()
    {
        bgBlack.gameObject.SetActive(false);
        bg.DOScale(Vector3.zero, animationTime).SetEase(easeOut);
    }
}
