public class PlayerAttackState : PlayerStateBase
{
    private enum ComboAnimation
    {
        None,
        Combo_04_1,
        Combo_04_2,
        Combo_04_3,
        Combo_04_4,
        Count,
    }
    private ComboAnimation currentCombo = ComboAnimation.None;
    private bool isSuperAttack = false;
    public PlayerAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        if (controller.player.enemy.GetComponent<TempEnemy>().isGroggy)
        {
            controller.player.animator.SetTrigger("SuperAttack");
            isSuperAttack = true;
            return;
        }

        currentCombo = ComboAnimation.None;
        controller.player.canCombo = true;
        SetCombo(ComboAnimation.Combo_04_1);
    }

    public override void Update()
    {
        var animatorStateInfo = controller.player.animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName(currentCombo.ToString()))
        {
            if (controller.player.canCombo && !controller.player.isAttack)
            {
                if (currentCombo == ComboAnimation.Count - 1)
                {
                    currentCombo = ComboAnimation.None;
                }
                SetCombo(currentCombo + 1);
            }
        }

        if (!TouchManager.Instance.Holded)
        {
            controller.SetState(PlayerController.State.Idle);
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        if (isSuperAttack)
        {
            controller.player.enemy.GetComponent<TempEnemy>().isGroggy = false;
            isSuperAttack = false;
        }
    }
    
    private void SetCombo(ComboAnimation newCombo)
    {
        if (currentCombo == newCombo)
        {
            return;
        }
        controller.player.animator.SetTrigger("Attack");
        currentCombo = newCombo;
    }
}
