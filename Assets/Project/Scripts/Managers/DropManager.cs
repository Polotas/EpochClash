using UnityEngine;

public static class DropManager 
{
    [Header("Dynamic Drop Settings")]
    public static int baseDropValue = 5;
    public static float bonusDropChance = 0.15f; // 15% chance
    public static int bonusDropMultiplier = 2; // 2x valor base
    public static int killStreakBonus = 0;
    public static int currentKillStreak = 0;
    
    public static void DropCurrency(Transform initPosition, bool isEliteEnemy = false)
    {
        GameObject dropObj = ObjectPooler.Instance
            .SpawnFromPool("CurrencyDrop", initPosition.position, Quaternion.identity);

        CurrencyDrop currencyDrop = dropObj.GetComponent<CurrencyDrop>();
        
        // Calcula valor do drop dinamicamente
        int dropValue = CalculateDropValue(isEliteEnemy);
        bool isBonusDrop = dropValue > baseDropValue;
        
        // Configura o drop
        currencyDrop.baseDropValue = baseDropValue;
        currencyDrop.bonusDropValue = dropValue - baseDropValue;
        currencyDrop.isBonusDrop = isBonusDrop;
        
        currencyDrop.Initialize(initPosition.position);
        
        // Atualiza kill streak
        currentKillStreak++;
        if (currentKillStreak >= 10)
        {
            killStreakBonus = Mathf.Min(killStreakBonus + 1, 5); // Máximo +5 gold
            currentKillStreak = 0;
        }
    }
    
    private static int CalculateDropValue(bool isEliteEnemy)
    {
        int finalValue = baseDropValue;
        
        // Bonus para inimigos elite
        if (isEliteEnemy)
        {
            finalValue *= 2;
        }
        
        // Chance de drop bonus aleatório
        if (Random.Range(0f, 1f) < bonusDropChance)
        {
            finalValue *= bonusDropMultiplier;
        }
        
        // Bonus de kill streak
        finalValue += killStreakBonus;
        
        // Bonus baseado na era atual
        int eraBonus = GameManager.Instance.currentEra;
        finalValue += eraBonus;
        
        return finalValue;
    }
    
    public static void ResetKillStreak()
    {
        currentKillStreak = 0;
        killStreakBonus = 0;
    }
    
    public static void RockFX(Transform initPosition) => ObjectPooler.Instance.SpawnFromPool("ParticleRock", initPosition.position, Quaternion.identity);
    
    public static void SmokeDie(Transform initPosition) => ObjectPooler.Instance.SpawnFromPool("Smoke", initPosition.position, Quaternion.identity);
}
