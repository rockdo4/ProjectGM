public abstract class PlayerStateBase : StateBase
{
    protected PlayerController controller;


    public PlayerStateBase(PlayerController controller)
    {
        this.controller = controller;
    }
}