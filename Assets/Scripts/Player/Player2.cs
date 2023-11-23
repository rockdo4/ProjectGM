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
        currentState = newState;
        stateManager?.ChangeState(states[(int)newState]);
    }
}