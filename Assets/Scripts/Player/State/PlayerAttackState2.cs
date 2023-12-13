using UnityEditor.Animations;
using UnityEngine;

public struct ActionData
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int ReadyFrameRate { get; set; }
    public int ExcutionFrameRate { get; set; }
    public int PostExcutionFrameRate { get; set; }
    public int FinalFrameRate { get; set; }
}

public class PlayerAttackState2 : PlayerStateBase
{
    private Animator animator;
    private const string triggerName = "Attack";

    public PlayerAttackState2(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Hand);
        animator = controller.player.Animator;

        animator.SetTrigger(triggerName);
    }

    public override void Update()
    {
        if (!TouchManager.Instance.Holded)
        {
            controller.SetState(PlayerController.State.Idle);
        }

        var currentAnimationInfo = animator.GetCurrentAnimatorStateInfo(0);
        var clip = animator.GetCurrentAnimatorClipInfo(0)[0];

        if (currentAnimationInfo.IsName(clip.clip.name) && currentAnimationInfo.normalizedTime >= 0)
        {
        }
    }

    private void NextAnimation()
    {
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        //controller.player.Animator.ResetTrigger(triggerName);
    }

    private AnimatorStateMachine GetStateMachine()
    {
        return null;
    }

    private void OnAnimationEnd()
    {

    }
}
