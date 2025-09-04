using UnityEngine;

public class Base : MonoBehaviour
{
    public bool isPlayer;
    public Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (!isPlayer) health.onDamage += DropCurrency;
        health.onDie += EndGame;
    }

    public void EndGame()
    {
        if (isPlayer)
        {
            GameManager.Instance.EndGame(false,false);
        }
        else
        {
            GameManager.Instance.EndGame(false,true);
        }
    }

    private void DropCurrency() => DropManager.DropCurrency(transform);
}