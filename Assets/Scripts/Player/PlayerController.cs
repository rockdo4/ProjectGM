using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player player { get; private set; }
    private TouchManager touchManager;

    public enum State
    {
        Idle,
        Attack,
        Evade,
        Sprint,
    }
    private StateManager stateManager = new StateManager();
    private List<StateBase> states = new List<StateBase>();
    public State currentState { get; private set; }

    // equip weapon test
    private Item equipWeapon = null;
    public Transform leftHand;
    public Transform rightHand;
    public ItemSO weaponSO;

    private void Awake()
    {
        player = GetComponent<Player>();
        touchManager = TouchManager.Instance;

        // equip weapon test
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        equipWeapon = PlayDataManager.data.Inventory.Find
            (i => i.instanceID == PlayDataManager.data.Equipment[Item.ItemType.Weapon]);

        StateInit();
    }

    private void Start()
    {
        weaponSO.MakeItem(equipWeapon, rightHand, player.anim);

        touchManager.SwipeListeners += OnSwipe;
        touchManager.HoldListeners += OnHold;
        touchManager.HoldEndListeners += HoldEnd;
    }

    private void OnDestroy()
    {
        touchManager.SwipeListeners -= OnSwipe;
        touchManager.HoldListeners -= OnHold;
        touchManager.HoldEndListeners -= HoldEnd;
    }

    private void Update()
    {
        stateManager?.Update();

        Vector3 relativePos = player.enemy.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        // Y축 회전을 무시한 새로운 로테이션을 설정
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);

        //피격 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.evadeTimer < player.stat.justEvadeTime)
            {
                player.evadePoint += player.stat.justEvadePoint;
            }
            else if (player.evadeTimer >= player.stat.justEvadeTime && player.evadeTimer < player.stat.evadeTime)
            {
                player.evadePoint += player.stat.evadePoint;
            }
            else
            {
                player.evadePoint += player.stat.hitEvadePoint;
            }
            player.evadePoint = Mathf.Clamp(player.evadePoint, (int)player.slider.minValue, (int)player.slider.maxValue);
            player.slider.value = player.evadePoint;
        }
    }

    private void FixedUpdate()
    {
        stateManager?.FixedUpdate();
    }

    #region Touch Event
    private void OnSwipe()
    {
        if (currentState == State.Attack)
        {
            return;
        }
        SetState(State.Evade);
    }
    private void OnHold()
    {
        if (currentState == State.Evade)
        {
            return;
        }

        if (player.DistanceToEnemy < player.attackRange)
        {
            SetState(State.Attack);
        }
        else
        {
            SetState(State.Sprint);
        }
    }
    private void HoldEnd()
    {
        player.canCombo = false;
        //SetState(State.Idle);
    }
    #endregion

    #region Animation Events
    private void BeforeAttack()
    {
        player.isAttack = false;
        player.canCombo = false;
    }
    private void Attack()
    {
        player.isAttack = true;
    }
    private void AfterAttack()
    {
        player.canCombo = true;
    }
    private void EndAttack()
    {
        Debug.Log("EndAttack");
        player.isAttack = false;
        player.canCombo = false;
    }
    #endregion

    public void SetState(State newState)
    {
        if (newState == currentState)
        {
            return;
        }
        Debug.Log($"--------- ChangeState: {newState} ---------");
        currentState = newState;
        stateManager?.ChangeState(states[(int)newState]);
    }

    private void StateInit()
    {
        states.Add(new PlayerIdleState(this));
        states.Add(new PlayerAttackState(this));
        states.Add(new PlayerEvadeState(this));
        states.Add(new PlayerSprintState(this));

        SetState(State.Idle);
    }
}