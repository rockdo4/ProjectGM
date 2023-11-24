using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private bool isAnimationPlaying = false;

    public PlayerAttackState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        if (Player.Instance.DistanceToEnemy - Player.Instance.attackRange > Player.Instance.EvadeDistance)
        {
            TouchManager.Instance.swipeDirection = TouchManager.SwipeDirection.Up;
            Player.Instance.SetState(Player.States.Evade);
            return;
        }

        Player.Instance.anim.SetTrigger("Attack");
        isAnimationPlaying = true;
    }

    public override void Update()
    {
        if (isAnimationPlaying)
        {
            var animatorStateInfo = Player.Instance.anim.GetCurrentAnimatorStateInfo(0);
            
            if (animatorStateInfo.normalizedTime >= Player.Instance.comboSuccessRate)
            {
                if (TouchManager.Instance.Holded)
                {
                    Player.Instance.anim.SetTrigger("Attack");
                }
                else if (animatorStateInfo.normalizedTime >= 1.0f)
                {
                    Player.Instance.SetState(Player.States.Idle);
                }
            }
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {

    }

}
