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

    public enum States
    {
        Idle,
        Attack,
        Evade,
    }
    private StateManager stateManager = new StateManager();
    private List<StateBase> states = new List<StateBase>();
    public States currentState { get; private set; }

    #region TestData
    public Slider slider;
    public Color evadeColor = Color.white;
    public Color evadeSuccessColor = Color.yellow;
    public Color justEvadeSuccessColor = Color.green;
    public Color hitColor = Color.red;
    public Color originalColor;
    public MeshRenderer ren { get; private set; }
    #endregion

    private void Awake()
    {
        Instance = this;

        ren = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        colldier = GetComponent<Collider>();
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>(); // animator test code
    }

    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag(Tags.enemy);
        //Look At Enemy
        CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.LookAt = enemy.transform;
        originalColor = ren.material.color;

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