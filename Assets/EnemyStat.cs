using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStat", menuName = "EnemyStat")]
public class EnemyStat : Stat
{
    [Header("약점 속성")]
    public AttackType weaknessType = AttackType.None;

    public override Attack CreateAttack(LivingObject attacker, LivingObject defender, bool groogy = false)
    {
        float damage = attacker.stat.AttackDamage;

        var critical = Random.value < attacker.stat.Critical;
        if (critical)
        {
            damage *= attacker.stat.CriticalDamage;
        }

        if (defender != null)
        {
            damage -= defender.stat.Defence;
            if (damage < 0)
            {
                damage = 0;
            }
        }

        return new Attack((int)damage, critical, groogy);
    }
}
