using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : LivingObject
{
    [HideInInspector]
    public float evadeTimer = 0f;
    public float evadePoint { get; set; } = 0;
    public bool GroggyAttack { get; set; }

    public enum AttackState
    {
        Before, Attack, AfterStart, AfterEnd, End
    }
    public AttackState attackState { get; set; }
    public bool canCombo { get; set; }
    public bool isAttack { get; set; }

    public bool IsHold { get; set; }

    public LivingObject Enemy { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public BoxCollider Colldier { get; private set; }
    [Header("가상 카메라 연결")]
    public CinemachineVirtualCamera virtualCamera;
    public Animator Animator { get; private set; }// animator test code
    public WeaponPrefab CurrentWeapon { get; set; }
    public WeaponPrefab FakeWeapon { get; set; }

    public PlayerEffects effects { get; private set; }

    public float MoveDistance
    {
        get
        {
            return Colldier.bounds.size.y * 2;
        }
    }
    public float DistanceToEnemy
    {
        get
        {
            if (Enemy == null)
            {
                return 0f;
            }
            return Vector3.Distance(transform.position, Enemy.transform.position);
        }
    }
    public bool CanAttack
    {
        get
        {
            if (Enemy == null || CurrentWeapon == null)
            {
                return false;
            }
            return DistanceToEnemy < CurrentWeapon.attackRange;
        }
    }

    public PlayerStat Stat
    {
        get
        {
            return stat as PlayerStat;
        }
    }

    public PlayerController.State CurrentState
    {
        get
        {
            return GetComponent<PlayerController>().currentState;
        }
    }

    #region TestData
    public Slider slider { get; private set; }
    public int comboCount { get; set; } = 0;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        Rigid = GetComponent<Rigidbody>();
        Colldier = GetComponent<BoxCollider>();
        Animator = GetComponent<Animator>();
        slider = GetComponentInChildren<Slider>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        effects = GetComponent<PlayerEffects>();
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