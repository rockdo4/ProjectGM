using UnityEngine;

public class PlayerAttackState2 : PlayerStateBase
{
    private Animator animator;
    private const string triggerName = "Attack";
    private float comboTimer = 0f;
    private float comboTime = 0.5f;

    private bool SetNextAttack = false;

    public PlayerAttackState2(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        animator ??= controller.player.Animator;
        animator.speed = controller.player.Stat.globalSpeed.attackSpeed * controller.player.Stat.attackSpeed;

        comboTimer = 0f;
        SetNextAttack = false;

        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Hand);

        animator.SetTrigger(triggerName);
    }

    public override void Update()
    {
        if (controller.player.Enemy.IsGroggy == true)
        {
            controller.SetState(PlayerController.State.Idle);
        }

        switch (controller.player.attackState)
        {
            case Player.AttackState.Before:
                comboTimer = 0f;
                SetNextAttack = false;
                break;
            case Player.AttackState.Attack:
                break;
            case Player.AttackState.AfterStart:
                if (!SetNextAttack && TouchManager.Instance.Holded)
                {
                    animator.SetTrigger(triggerName);
                    SetNextAttack = true;
                }
                break;
            case Player.AttackState.AfterEnd:
                break;
            case Player.AttackState.End:
                comboTimer += Time.deltaTime;
                if (comboTimer >= comboTime)
                {
                    controller.SetState(PlayerController.State.Idle);
                }
                break;
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        controller.player.attackState = Player.AttackState.None;
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
