using UnityEngine;

public class PlayerAttackState2 : PlayerStateBase
{
    private Animator animator;
    private const string triggerName = "Attack";
    private float comboTimer = 0f;
    private float comboTime = 0.5f;

    public PlayerAttackState2(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        comboTimer = 0f;
        animator = controller.player.Animator;

        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Hand);

        animator.SetTrigger(triggerName);
    }

    public override void Update()
    {
        if (controller.player.Enemy.IsGroggy == true)
        {
            controller.SetState(PlayerController.State.SuperAttack);
        }

        switch (controller.player.attackState)
        {
            case Player.AttackState.Before:
                //선딜
                //회피 가능
                break;
            case Player.AttackState.Attack:
                //공격판정
                //회피 불가
                break;
            case Player.AttackState.AfterStart:
                //후딜 시작
                //행동 불가능
                //선입력 시작
                if (TouchManager.Instance.Holded)
                {
                    animator.SetTrigger(triggerName);
                }
                break;
            case Player.AttackState.AfterEnd:
                //후딜 종료
                //회피 가능
                if (TouchManager.Instance.Holded)
                {
                    animator.SetTrigger(triggerName);
                }
                break;
            case Player.AttackState.End:
                //애니메이션 종료
                if (!animator.GetBool(triggerName))
                {
                    comboTimer += Time.deltaTime;
                    Debug.Log(comboTimer);
                    if (comboTimer >= comboTime)
                    {
                        controller.SetState(PlayerController.State.Idle);
                    }
                }
                break;
        }

        Debug.Log(controller.player.attackState.ToString());
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
