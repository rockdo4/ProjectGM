using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2 : MonoBehaviour
{
    [Header("PlayerStat ¿¬°á")]
    public PlayerStat stat;

    public static Player2 Instance;

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
        Debug.Log(currentState.ToString());
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
    private void Attack()
    {
        IsAttack = true;
        Debug.Log("Attack");
    }

    private void Combo()
    {
        IsAttack = false;
        //if (comboSuccess)
        //{
        //    comboSuccess = false;
        //    IsAttack = false;
        //    if (comboCount < maxComboCount)
        //    {
        //        anim.SetInteger("NewAttack", ++comboCount);
        //    }
        //}
        //else
        //{
        //    anim.SetInteger("NewAttack", comboCount = 0);
        //}
    }
    #endregion

}