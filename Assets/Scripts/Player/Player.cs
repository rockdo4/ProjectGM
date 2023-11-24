using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("PlayerStat 연결")]
    public PlayerStat stat;

    public static Player Instance;

    public float evadeTimer = 0f;
    public int evadePoint = 0;

    public GameObject enemy { get; private set; }
    public Rigidbody rigid { get; private set; }
    public Collider colldier { get; private set; }
    public PlayerController playerController { get; private set; }
    public Animator anim; // animator test code

    public CinemachineVirtualCamera virtualCamera;

    public enum States
    {
        Idle,
        Attack,
        Evade,
    }
    private StateManager stateManager = new StateManager();
    private List<StateBase> states = new List<StateBase>();
    public States currentState { get; private set; }

    public float EvadeDistance
    {
        get
        {
            return colldier.bounds.size.y * 2;
        }
    }
    public float DistanceToEnemy
    {
        get
        {
            if (enemy == null)
            {
                return 0f;
            }
            return Vector3.Distance(transform.position, enemy.transform.position);
        }
    }

    #region TestData
    public Slider slider;
    public float attackRange = 2f;
    public int maxComboCount = 4;
    public int comboCount = 0;
    public bool comboSuccess = false;
    public float comboSuccessRate = 0.8f;
    public bool IsAttack { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;

        rigid = GetComponent<Rigidbody>();
        colldier = GetComponent<Collider>();
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>(); // animator test code
    }

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag(Tags.enemy);

        //Look At Enemy
        virtualCamera.LookAt = enemy.transform;
        
        states.Add(new PlayerIdleState(playerController));
        states.Add(new PlayerAttackState(playerController));
        states.Add(new PlayerEvadeState(playerController));

        SetState(States.Idle);
    }

    private void Update()
    {
        stateManager?.Update();
    }

    private void FixedUpdate()
    {
        rigid.transform.LookAt(enemy.transform);

        stateManager?.FixedUpdate();
    }

    public void SetState(States newState)
    {
        if (newState == currentState)
        {
            return;
        }
        currentState = newState;
        stateManager?.ChangeState(states[(int)newState]);
    }

    #region Animation Events
    private void BeforeAttack()
    {
        IsAttack = false;
        Debug.Log($"{Time.time} : 선딜레이");
    }
    private void Attack()
    {
        Debug.Log($"{Time.time} : 선딜레이 끝");
        IsAttack = true;
        Debug.Log($"{Time.time} : 공격판정");

        Debug.Log($"{Time.time} : 후딜레이 시작");
    }
    private void AfterAttack()
    {
        IsAttack = false;
        Debug.Log($"{Time.time} : 후딜레이 끝");
    }
    #endregion

}