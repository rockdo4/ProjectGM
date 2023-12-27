using Cinemachine;
using UnityEngine;

public class Player : LivingObject
{
    [HideInInspector]
    public float evadeTimer = 0f;
    public float evadePoint { get; set; } = 0;
    public bool GroggyAttack { get; set; }

    public enum AttackState
    {
        None, Before, Attack, AfterStart, AfterEnd, End
    }
    public AttackState attackState { get; set; }
    public bool canCombo { get; set; }
    public bool isAttack { get; set; }

    public bool IsHold { get; set; }

    public LivingObject Enemy { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public CinemachineVirtualCamera VirtualCamera { get; private set; }
    public Animator Animator { get; private set; }// animator test code
    public WeaponPrefab CurrentWeapon { get; set; }
    public WeaponPrefab FakeWeapon { get; set; }

    public PlayerEffects Effects { get; private set; }

    public bool CanAttack
    {
        get
        {
            if (Enemy == null || CurrentWeapon == null)
            {
                return false;
            }

            Vector3 direction = Enemy.transform.position - transform.position;
            return direction.magnitude < CurrentWeapon.attackRange;
        }
    }

    public PlayerStat Stat
    {
        get
        {
            return stat as PlayerStat;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Rigid = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        VirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        Effects = GetComponent<PlayerEffects>();
    }

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag(Tags.enemy) == null)
        {
            return;
        }
        Enemy = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<LivingObject>();
    }
}