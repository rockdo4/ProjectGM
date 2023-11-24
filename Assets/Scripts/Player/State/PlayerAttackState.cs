using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private bool isAnimationPlaying = false;

    public PlayerAttackState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        if (Player2.Instance.DistanceToEnemy - Player2.Instance.attackRange > Player2.Instance.EvadeDistance)
        {
            TouchManager.Instance.swipeDirection = TouchManager.SwipeDirection.Up;
            Player2.Instance.SetState(Player2.States.Evade);
            return;
        }

        Player2.Instance.anim.SetTrigger("Attack");
        isAnimationPlaying = true;
    }

    public override void Update()
    {
        if (isAnimationPlaying)
        {
            var animatorStateInfo = Player2.Instance.anim.GetCurrentAnimatorStateInfo(0);
            
            if (animatorStateInfo.normalizedTime >= Player2.Instance.comboSuccessRate)
            {
                if (TouchManager.Instance.Taped || TouchManager.Instance.Holded)
                {
                    Player2.Instance.anim.SetTrigger("Attack");
                }
                else if (animatorStateInfo.normalizedTime >= 1.0f)
                {
                    Player2.Instance.SetState(Player2.States.Idle);
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
