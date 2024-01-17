using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "PlayerStat")]
public class PlayerStat : Stat
{
    [System.Serializable]
    public class GlobalAnimationSpeed
    {
        [Header("회피")]
        [Range(0.1f, 5)]
        public float evadeSpeed = 1f;
        [Header("공격")]
        [Range(0.1f, 5)]
        public float attackSpeed = 1f;
        [Header("특수공격")]
        [Range(0.1f, 5)]
        public float superAttackSpeed = 1f;
        [Header("돌진")]
        [Range(0.1f, 5)]
        public float sprintSpeed = 1f;
    }

    [Header("기본 애니메이션 속도 배율")]
    [SerializeField]
    public GlobalAnimationSpeed globalSpeed;

    [Header("좌우 회피 거리")]
    public float leftRightEvadeDistance = 2f;

    [Header("전후 회피 거리")]
    public float frontBackEvadeDistance = 2f;

    [Header("접근 거리")]
    public float moveDistance = 2f;

    [Header("회피 판정 시간(sec)")]
    public float evadeTime;

    [Header("그로기 공격에 필요한 회피 포인트")]
    public float maxEvadePoint;
    
    [Header("회피 시 회피 포인트")]
    [Range(-100, 100)]
    public float evadePoint;

    [Header("회피 시 피격 대미지 배율")]
    [Range(0f, 3f)]
    public float evadeDamageRate;

    [Header("저스트 회피 판정 시간(sec)")]
    public float justEvadeTime;

    [Header("저스트 회피 시 회피 포인트")]
    [Range(-100, 100)] 
    public float justEvadePoint;

    [Header("피격 시 회피 포인트")]
    [Range(-100, 100)]
    public float hitEvadePoint;

    [Header("그로기 유발 시간(sec)")]
    public float groggyTime;

    [Header("공격 속도")]
    [Range(0.1f, 5f)]
    public float attackSpeed = 1f;

    [Header("특수 공격 배율")]
    public float superAttackRate = 3f;

    [Header("피격시 대미지 상쇄 확률")]
    [Range(0f, 1f)]
    public float block = 0f;

    [Header("최종 대미지 배율")]
    [Range(0f, 5f)]
    public float attackFinalRate = 0f;

    [Header("공격 시 회피 포인트 증가 배율")]
    [Range(0f, 1f)]
    public float attackEvadePointRate = 0f;

    [Header("공격 시 흡혈 비율")]
    public float drainRate = 0f;

    public override Attack CreateAttack(LivingObject attacker, LivingObject defender, bool groggy)
    {
        var player = attacker as Player;
        var enemyStat = defender.stat as EnemyStat;
        float damage = player.Stat.AttackDamage + player.CurrentWeapon.attack;

        if (enemyStat.weaknessType == player.CurrentWeapon.type)
        {
            damage += (damage * (player.CurrentWeapon.weakDamage));
        }

        if (player.GetComponent<PlayerController>().CurrentState == PlayerController.State.SuperAttack)
        {
            damage *= superAttackRate;
        }

        var critical = Random.value < player.Stat.Critical;
        if (critical)
        {
            damage *= player.Stat.CriticalDamage;
        }

        damage += damage * attackFinalRate;

        if (enemyStat != null)
        {
            damage -= enemyStat.Defence;
            if (damage < 0)
            {
                damage = 0;
            }
        }

        if (damage > 0)
        {
            player.HP += (int)(damage * player.Stat.drainRate);
        }

        return new Attack((int)damage, critical, groggy);
    }
}
