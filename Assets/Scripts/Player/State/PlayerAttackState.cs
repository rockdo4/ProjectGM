using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        }

        Player2.Instance.comboSuccess = false;
        Player2.Instance.comboCount = 1;

        Player2.Instance.anim.SetInteger("NewAttack", Player2.Instance.comboCount);
        isAnimationPlaying = true;
    }

    public override void Update()
    {
        if (Player2.Instance.comboCount == Player2.Instance.maxComboCount)
        {
            Player2.Instance.comboCount = 1;
        }

        if (isAnimationPlaying)
        {
            var animatorStateInfo = Player2.Instance.anim.GetCurrentAnimatorStateInfo(0);

            if (animatorStateInfo.normalizedTime >= Player2.Instance.comboSuccessRate)
            {
                if (TouchManager.Instance.Taped || TouchManager.Instance.Holded)
                {
                    Debug.Log(Player2.Instance.comboSuccess);
                    Player2.Instance.comboSuccess = true;
                    Player2.Instance.anim.SetInteger("NewAttack", ++Player2.Instance.comboCount);
                }
            }

            if (animatorStateInfo.normalizedTime >= 1.0f && !Player2.Instance.comboSuccess)
            {
                Player2.Instance.SetState(Player2.States.Idle);
            }
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        Player2.Instance.anim.SetInteger("NewAttack", Player2.Instance.comboCount = 0);
    }

}
