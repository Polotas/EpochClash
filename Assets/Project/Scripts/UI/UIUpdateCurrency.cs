using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Currency_Type
{
    Gold,
    Meat,
    GoldEarned
}

public class UIUpdateCurrency : MonoBehaviour
{
    public Currency_Type currencyType;
    public Image visual;
    public TextMeshProUGUI text;
    
    private void Start()
    {
        switch (currencyType)
        {
            case Currency_Type.Gold:
                GameManager.Instance.onCurrencyChanged += UpdateView;
                break;
            case Currency_Type.Meat:
                MeatManager.Instance.OnMeatChanged += UpdateView;
                break;
            case Currency_Type.GoldEarned:
                GameManager.Instance.onCurrencyEarnedChanged += UpdateView;
                break;
        }
    }

    private void UpdateView(int valor)
    {
        var punch = new Vector3(0.2f, 0.2f, 0.2f);
        text.transform.localScale = Vector3.one;
        
        // Usa o sistema seguro de animação
        var safeAnimator = this.GetSafeAnimator();
        safeAnimator.SafePunchScale(visual.transform, punch, 0.2f, "visual_punch");
        safeAnimator.SafePunchScale(text.transform, punch, 0.2f, "text_punch");
        
        text.text = CurrencyFormatter.FormatCurrency(valor);
        
        if(currencyType == Currency_Type.GoldEarned) AudioManager.PlayCollectCoin();
    }
}