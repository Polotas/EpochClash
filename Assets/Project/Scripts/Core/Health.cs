using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public int currentHealth;
    public Action<int,int,int> onUpdateHealth;
    public Action onDamage;
    public Action onDie;
    public bool IsDead; //{ get; private set; }
    
    public void OnEnable()
    {
        currentHealth = health;
        onUpdateHealth?.Invoke(health,currentHealth,0);
        IsDead = false;
    }

    public void Reset()
    {
        currentHealth = health;
        onUpdateHealth?.Invoke(health,currentHealth,0);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        onUpdateHealth?.Invoke(health,currentHealth,damage);
        onDamage?.Invoke();
        
        if (currentHealth <= 0)
        {
            IsDead = true;
            onUpdateHealth?.Invoke(1,1,0);
            onDie?.Invoke();
        }
    }
    
}
