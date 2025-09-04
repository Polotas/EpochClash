using UnityEngine;

public static class DropManager 
{
    public static void DropCurrency(Transform initPosition)
    {
        GameObject dropObj = ObjectPooler.Instance
            .SpawnFromPool("CurrencyDrop", initPosition.position, Quaternion.identity);

        dropObj.GetComponent<CurrencyDrop>()
            .Initialize(initPosition.position);
    }
    
    public static void RockFX(Transform initPosition) => ObjectPooler.Instance.SpawnFromPool("ParticleRock", initPosition.position, Quaternion.identity);
    
    public static void SmokeDie(Transform initPosition) => ObjectPooler.Instance.SpawnFromPool("Smoke", initPosition.position, Quaternion.identity);
}
