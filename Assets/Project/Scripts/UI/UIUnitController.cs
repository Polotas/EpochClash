using System;
using DG.Tweening;
using UnityEngine;

public class UIUnitController : MonoBehaviour
{
    public UIUnit[] buttons;
    public Transform bottomMenu;
    public Transform topMenu;
    
    public float bottomShow;
    public float bottomHide;
    public float topShow;
    public float topHide;

    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    [ContextMenu("SHOW")]
    public void Show() => ShowOrHideBottomMenu(true);

    [ContextMenu("HIDE")]
    public void Hide() => ShowOrHideBottomMenu(false);
    
    public void ShowOrHideBottomMenu(bool active)
    {
        if (active)
        {
            foreach (var t in buttons)
                t.Setup();
            
            bottomMenu.DOLocalMoveY(bottomShow, 0.5f).SetEase(Ease.InBack);
            topMenu.DOLocalMoveY(topShow, 0.5f).SetEase(Ease.InBack);
            
            buttons[1].gameObject.SetActive(_gm.saveUpgrade.unlockUnit2);
            buttons[2].gameObject.SetActive(_gm.saveUpgrade.unlockUnit3);
        }
        else
        {
            bottomMenu.DOLocalMoveY(bottomHide, 0.5f).SetEase(Ease.InOutBack);
            topMenu.DOLocalMoveY(topHide, 0.5f).SetEase(Ease.InOutBack);
        }
    }
}
