using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "PlayerStat")]
public class PlayerStat : Stat
{
    [Header("회피 판정 시간")]
    public float evadeTime;

    [Header("저스트 회피 판정 시간")]
    public float evadeJustTime;

    [Header("피격 시 무적 시간")]
    public float hitInvincibleTime;
}
