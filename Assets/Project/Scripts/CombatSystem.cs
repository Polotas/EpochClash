using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static void Attack(UnitController attacker, UnitController target)
    {
        if (target == null || target.health.IsDead) return;
        target.health.TakeDamage(attacker.currentUnit.attackDamage);
    }

    public static void AttackRanged(int damage, UnitController target)
    {
        if (target == null || target.health.IsDead) return;
        target.health.TakeDamage(damage);
    }
}