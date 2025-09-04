using UnityEngine;

public class UIPorgressBar : MonoBehaviour
{
    public Currency_Type currencyType;
    public Transform progress;

    private void Start()
    {
        switch (currencyType)
        {
            case Currency_Type.Gold:
               // GameManager.Instance.onCurrencyChanged += UpdateView;
                break;
            case Currency_Type.Meat:
                MeatManager.Instance.OnMeatProgress += UpdateView;
                break;
            case Currency_Type.GoldEarned:
               // GameManager.Instance.onCurrencyEarnedChanged += UpdateView;
                break;
        }
    }

    private void UpdateView(float valor) => progress.localScale = new Vector3(valor, 1, 1);
}
