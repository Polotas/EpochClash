using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "EpochClash/Era_Data", order = 1)]

public class EnemyEra : ScriptableObject
{
    public EraAge[] eraAges;
}

[Serializable]
public class EraAge
{
    public GameEra currentEra = GameEra.StoneAge;
    public AgeRounds[] ageRounds;
}

[Serializable]
public class AgeRounds
{
    public float delayBetweenWaves;
    public RoundsWave[] roundsWaves;
}

[Serializable]
public class RoundsWave
{
    public float delayBetweenRounds;
    public int quantity;
    public WarriorType warriorType;
}