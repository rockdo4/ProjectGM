public class PlayerHitState : PlayerStateBase
{
    private const string triggerName = "Hit";

    public PlayerHitState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.player.Animator.Play(triggerName);
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        controller.player.IsGroggy = false;
    }
}
