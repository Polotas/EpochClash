using TMPro;
using UnityEngine;

public class UIHeathBar : MonoBehaviour
{
    public bool isPlayer;
    public bool activeText;
    public GameObject objBar;
    public Transform heathBar;
    public Health health;
    public TextMeshProUGUI textHeath;

    public Color damageColor;

    private void Start()
    {
        health.onUpdateHealth += UpdateBar;
        GameManager.Instance.onInitGame += InitGame;
        GameManager.Instance.onEndGame += EndGame;
    }

    private void InitGame()
    {
        health.Reset();
        textHeath.gameObject.SetActive(activeText);
    }
    
    private void EndGame()
    {
        textHeath.gameObject.SetActive(false);
    }

    private void UpdateBar(int maxHp, int currentHp, int currentDamage)
    {
        var isResetBar = maxHp == currentHp;

        textHeath.text = CurrencyFormatter.FormatCurrency(currentHp);
        var current = (float)currentHp / (float)maxHp;
        heathBar.transform.localScale = new Vector3(current, 1, 1);
        
        objBar.SetActive(!isResetBar);
        if (isResetBar) return;
        
        Vector3 spawnPos = transform.position + Vector3.up;

        GameObject dmgTextObj = ObjectPooler.Instance
            .SpawnFromPool("DamageText", spawnPos, Quaternion.identity);

        dmgTextObj.GetComponent<TextDamage>()
            .Initialize(currentDamage,isPlayer,transform.position , damageColor);
    }
}
