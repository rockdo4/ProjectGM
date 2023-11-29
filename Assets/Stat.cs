using UnityEngine;

[CreateAssetMenu(menuName = "Stat", fileName = "DefaultStat")]
public class Stat : ScriptableObject
{
    [Header("초기 체력")]
    public int HP;

    [Header("공격력")]
    public int AttackDamage;

    [Header("방어력")]
    public int Defence;

    [Header("이동속도")]
    public float MoveSpeed;

    [Header("치명타 확률")]
    public float Critical;

    [Header("치명타 배율")]
    public float CriticalDamage;

    public virtual Attack CreateAttack(LivingObject attacker, LivingObject defender, bool groogy = false)
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
        }

        return new Attack((int)damage, critical, groogy);
    }
}