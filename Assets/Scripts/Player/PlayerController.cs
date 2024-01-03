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
        Hit,
        Death
    }
    public enum EvadeState
    {
        None, Normal, Just
    }
    private StateManager stateManager = new StateManager();
    private List<StateBase> states = new List<StateBase>();
    public State CurrentState { get; private set; }
    public State NextState { get; set; }
    public EvadeState LastEvadeState { get; set; }
    #region Weapon
    public enum WeaponPosition
    {
        Hand, Wing
    }
    public WeaponPosition currentWeaponPosition { get; private set; }
    private Weapon equipWeapon = null;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftWing;
    public Transform rightWing;
    public WeaponSO weaponSO;
    #endregion

    #region IK
    private Transform subHandle;
    #endregion

    private void Awake()
    {
        player = GetComponent<Player>();
        touchManager = TouchManager.Instance;

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        equipWeapon = PlayDataManager.curWeapon;

        // Defence Reset
        player.Stat.Defence = 0;
        foreach (var armor in PlayDataManager.curArmor)
        {
            if (armor.Value != null)
            {
                var table = CsvTableMgr.GetTable<ArmorTable>().dataTable;
                player.Stat.Defence += table[armor.Value.id].defence;
            }
        }

        StateInit();
    }

    private void Start()
    {
        player.CurrentWeapon = weaponSO.MakeWeapon(equipWeapon, rightHand, player.Animator);
        if (equipWeapon.weaponType == WeaponType.Tonpa)
        {
            player.FakeWeapon = Instantiate(player.CurrentWeapon, leftHand);
        }
        subHandle = player.CurrentWeapon.transform.Find("LeftHandle");

        //MoveWeaponPosition(WeaponPosition.Wing);
        
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
        if (player.Enemy == null)
        {
            return;
        }
        if (CurrentState == State.Death)
        {
            return;
        }
        if (player.HP <= 0)
        {
            SetState(State.Death);
        }
        stateManager?.Update();
        Vector3 relativePos = player.Enemy.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);

        if (player.IsGroggy)
        {
            SetState(State.Hit);
        }

        #region EvadePoint
        if (player.evadePoint >= player.Stat.maxEvadePoint)
        {
            player.GroggyAttack = true;
        }
        if (player.GroggyAttack && !player.Enemy.IsGroggy)
        {
            player.evadePoint -= Time.deltaTime * (player.Stat.maxEvadePoint / player.Stat.groggyTime);
            if (player.evadePoint <= 0f)
            {
                player.evadePoint = 0f;
                player.GroggyAttack = false;
            }
        }
        #endregion

        #region Test Input
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.evadePoint += 50;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.Stat.AttackDamage = (player.Stat.AttackDamage == 0) ? 70 : 0;
            Debug.Log(player.Stat.AttackDamage);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.Stat.Defence = (player.Stat.Defence == 0) ? -100 : 0;
            Debug.Log(player.Stat.Defence);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        stateManager?.FixedUpdate();
    }

    #region Touch Event
    private void OnSwipe()
    {
        if (CurrentState == State.Hit || CurrentState == State.Death)
        {
            NextState = State.Evade;
            return;
        }
        if (player.attackState == Player.AttackState.AfterStart || CurrentState == State.SuperAttack)
        {
            return;
        }

        SetState(State.Evade);
    }
    private void OnHold()
    {
        if (CurrentState != State.Idle)
        {
            NextState = State.Sprint;
            return;
        }

        SetState(State.Sprint);
    }
    private void HoldEnd()
    {

    }
    #endregion

    #region Animation Event
    private void BeforeAttack()
    {
        player.attackState = Player.AttackState.Before;
        if (CurrentState == State.SuperAttack)
        {
            switch (player.CurrentWeapon.weaponType)
            {
                case WeaponType.Tonpa:
                    break;
                case WeaponType.Two_Hand_Sword:
                    player.Effects.PlayEffect(PlayerEffectType.Super_TwoHandSword_Charge);
                    break;
                case WeaponType.One_Hand_Sword:
                    break;
                case WeaponType.Spear:
                    player.Effects.PlayEffect(PlayerEffectType.Super_Spear);
                    break;
            }
        }
    }
    private void Attack()
    {
        player.attackState = Player.AttackState.Attack;
        if (player.CurrentWeapon == null)
        {
            return;
        }
        
        if (CurrentState == State.SuperAttack)
        {
            switch (player.CurrentWeapon.weaponType)
            {
                case WeaponType.Tonpa:
                    player.Effects.PlayEffect(PlayerEffectType.Super_Tonpa);
                    break;
                case WeaponType.Two_Hand_Sword:
                    player.Effects.StopEffect(PlayerEffectType.Super_TwoHandSword_Charge);
                    player.Effects.PlayEffect(PlayerEffectType.Super_TwoHandSword);
                    break;
                case WeaponType.One_Hand_Sword:
                    player.Effects.PlayEffect(PlayerEffectType.Super_OneHandSword);
                    break;
                case WeaponType.Spear:
                    break;
            }
            player.Effects.PlayEffect(PlayerEffectType.SlowMotion);
        }
        else
        {
            player.Effects.PlayEffect(PlayerEffectType.Attack);
        }
        player.evadePoint += player.Stat.maxEvadePoint * player.Stat.attackEvadePointRate;
        ExecuteAttack(player, player.Enemy);
        player.attackState = Player.AttackState.AfterStart;
    }
    private void AfterAttack()
    {
        player.attackState = Player.AttackState.AfterEnd;
    }
    private void EndAttack()
    {
        player.attackState = Player.AttackState.End;
    }
    private void EndAnimationDefault()
    {
        SetState(State.Idle);
    }
    #endregion

    public void SetState(State newState)
    {
        if (newState == CurrentState || CurrentState == State.Death)
        {
            return;
        }
        CurrentState = newState;
        NextState = State.Idle;
        stateManager?.ChangeState(states[(int)newState]);
    }

    private void StateInit()
    {
        states.Add(new PlayerIdleState(this));
        states.Add(new PlayerAttackState2(this));
        states.Add(new PlayerSuperAttackState(this));
        states.Add(new PlayerEvadeState(this));
        states.Add(new PlayerSprintState(this));
        states.Add(new PlayerHitState(this));
        states.Add(new PlayerDeathState(this));

        SetState(State.Idle);
    }

    private void ExecuteAttack(LivingObject attacker, LivingObject defender)
    {
        Attack attack = player.Stat.CreateAttack(attacker, defender, player.GroggyAttack);
        if (!player.Enemy.IsGroggy && player.GroggyAttack)
        {
            player.evadePoint = 0f;
        }

        var attackables = defender.GetComponents<IAttackable>();
        player.Effects.PlayEffect(PlayerEffectType.Attack);

        foreach (var attackable in attackables)
        {
            attackable.OnAttack(player.gameObject, attack);
        }


    }

    public void MoveWeaponPosition(WeaponPosition position)
    {
        return;
        //currentWeaponPosition = position;
        //switch (position)
        //{
        //    case WeaponPosition.Hand:
        //        player.CurrentWeapon.transform.SetParent(rightHand, false);
        //        player.FakeWeapon?.transform.SetParent(leftHand, false);
        //        break;
        //    case WeaponPosition.Wing:
        //        player.CurrentWeapon.transform.SetParent(rightWing, false);
        //        player.FakeWeapon?.transform.SetParent(leftWing, false);
        //        break;
        //}
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (subHandle == null || currentWeaponPosition != WeaponPosition.Hand)
        {
            return;
        }
        player.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        player.Animator.SetIKPosition(AvatarIKGoal.LeftHand, subHandle.transform.position);
    }

    public void GroggyAttack()
    {
        if (player.evadePoint < player.Stat.maxEvadePoint)
        {
            return;
        }
        player.GroggyAttack = true;
        player.evadePoint = 0f;
    }

    public void CheckNextState()
    {
        if (NextState == State.Idle)
        {
            return;
        }
        SetState(NextState);
    }
}