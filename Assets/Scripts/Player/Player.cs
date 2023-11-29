using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : LivingObject
{
    public float evadeTimer = 0f;
    public float evadePoint { get; set; } = 0;
    public bool GroggyAttack { get; set; }

    public bool canCombo { get; set; }
    public bool isAttack { get; set; }

    public TempEnemy Enemy { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public BoxCollider Colldier { get; private set; }
    public CinemachineVirtualCamera virtualCamera;
    public Animator Animator { get; private set; }// animator test code
    public Weapon CurrentWeapon { get; set; }

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

    #region TestData
    public Slider slider;
    public int comboCount { get; set; } = 0;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        Rigid = GetComponent<Rigidbody>();
        Colldier = GetComponent<BoxCollider>();
        Animator = GetComponent<Animator>(); // animator test code
    }

    private void Start()
    {
        Enemy = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<TempEnemy>();
        virtualCamera.Follow = transform;
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;

        Debug.Log("�÷��̾� HP : " + HP);
    }
}