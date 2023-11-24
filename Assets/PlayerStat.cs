using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "PlayerStat")]
public class PlayerStat : Stat
{
    [Header("회피 판정 시간(sec)")]
    public float evadeTime;
    
    [Header("회피 시 회피 포인트")]
    [Range(-100, 100)]
    public int evadePoint;

    [Header("회피 시 대미지 감소 비율")]
    [Range(0f, 1f)]
    public float evadeDamageRate;

    [Header("저스트 회피 판정 시간(sec)")]
    public float justEvadeTime;

    [Header("저스트 회피 시 회피 포인트")]
    [Range(-100, 100)]
    public int justEvadePoint;

    [Header("피격 시 회피 포인트")]
    [Range(-100, 100)]
    public int hitEvadePoint;

    [Header("피격 시 무적 시간(sec)")]
    public float hitInvincibleTime;
}
