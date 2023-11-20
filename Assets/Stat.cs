using UnityEngine;

[CreateAssetMenu(menuName = "Stat", fileName = "DefaultStat")]
public class Stat : ScriptableObject
{
    [Header("체력")]
    public float HP;

    [Header("공격력")]
    public float AttackDamage;

    [Header("방어력")]
    public float Defence;

    [Header("이동속도")]
    public float MoveSpeed;

    [Header("치명타 확률")]
    public float Critical;

    [Header("치명타 배율")]
    public float CriticalDamage;

}