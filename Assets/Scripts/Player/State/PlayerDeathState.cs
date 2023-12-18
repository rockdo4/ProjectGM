public class PlayerDeathState : PlayerStateBase
{
    private const string triggerName = "Die";
    public PlayerDeathState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.player.Animator.Play(triggerName);
        controller.player.virtualCamera.enabled = false;
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
    }
}
