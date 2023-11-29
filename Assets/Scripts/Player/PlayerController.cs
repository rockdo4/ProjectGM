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
        SuperAttack,
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
        player.CurrentWeapon = weaponSO.MakeItem(equipWeapon, rightHand, player.Animator);

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
        if (player.Enemy == null)
        {
            return;
        }
        Vector3 relativePos = player.Enemy.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);

        #region Test
        //피격 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.evadeTimer < player.Stat.justEvadeTime)
            {
                player.evadePoint += player.Stat.justEvadePoint;
            }
            else if (player.evadeTimer >= player.Stat.justEvadeTime && player.evadeTimer < player.Stat.evadeTime)
            {
                player.evadePoint += player.Stat.evadePoint;
            }
            else
            {
                player.evadePoint += player.Stat.hitEvadePoint;
            }
            player.evadePoint = Mathf.Clamp(player.evadePoint, player.slider.minValue, player.slider.maxValue);
        }

        //Groggy
        if (player.evadePoint >= player.Stat.maxEvadePoint)
        {
            player.GroggyAttack = true;
        }
        if (player.GroggyAttack)
        {
            player.evadePoint -= Time.deltaTime * (player.Stat.maxEvadePoint / player.Stat.groggyTime);
            if (player.evadePoint <= 0f)
            {
                player.evadePoint = 0f;
                player.GroggyAttack = false;
            }
        }
        player.slider.value = player.evadePoint;
        #endregion  
    }

    private void FixedUpdate()
    {
        stateManager?.FixedUpdate();
    }

    #region Touch Event
    private void OnSwipe()
    {
        if (player.isAttack)
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

        if (player.DistanceToEnemy < player.CurrentWeapon.attackRange && player.Enemy != null)
        {
            SetState((player.Enemy.IsGroggy) ? State.SuperAttack : State.Attack);
        }
        else
        {
            SetState(State.Sprint);
        }
    }
    private void HoldEnd()
    {
        player.canCombo = false;
    }
    #endregion

    #region Animation Event
    private void BeforeAttack()
    {
        player.isAttack = false;
        player.canCombo = false;
    }
    private void Attack()
    {
        player.isAttack = true;

        if (player.CurrentWeapon == null)
        {
            return;
        }

        if (player.DistanceToEnemy < player.CurrentWeapon.attackRange)
        {
            ExecuteAttack(player, player.Enemy);
            if (player.GroggyAttack)
            {
                player.GroggyAttack = false;
                player.evadePoint = 0f;
            }
        }
    }
    private void AfterAttack()
    {
        player.isAttack = false;
        player.canCombo = true;
    }
    private void EndAttack()
    {
        player.canCombo = false;
        SetState(State.Idle);
    }
    #endregion

    public void SetState(State newState)
    {
        if (newState == currentState)
        {
            return;
        }
#if UNITY_EDITOR
        Debug.Log($"--------- ChangeState: {newState} ---------");
#endif
        currentState = newState;
        stateManager?.ChangeState(states[(int)newState]);
    }

    private void StateInit()
    {
        states.Add(new PlayerIdleState(this));
        states.Add(new PlayerAttackState(this));
        states.Add(new PlayerSuperAttackState(this));
        states.Add(new PlayerEvadeState(this));
        states.Add(new PlayerSprintState(this));

        SetState(State.Idle);
    }

    private void ExecuteAttack(LivingObject attacker, LivingObject defender)
    {
        Attack attack = player.Stat.CreateAttack(attacker, defender, player.GroggyAttack);

        var attackables = defender.GetComponents<IAttackable>();
        foreach (var attackable in attackables)
        {
            attackable.OnAttack(player.gameObject, attack);
        }
    }
}