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
    private bool isFirst = false;

    public PlayerAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        if (controller.player.enemy.GetComponent<TempEnemy>().isGroggy)
        {
            controller.player.isAttack = true;
            controller.player.canCombo = true;
            controller.player.animator.SetTrigger("SuperAttack");
            isSuperAttack = true;
            return;
        }

        currentCombo = ComboAnimation.None;
        isFirst = true;
        //SetCombo(ComboAnimation.Combo_04_1);
    }

    public override void Update()
    {
        if (!TouchManager.Instance.Holded)
        {
            controller.SetState(PlayerController.State.Idle);
        }

        if (currentCombo == ComboAnimation.None || (controller.player.canCombo && !controller.player.isAttack))
        {
            if (currentCombo == ComboAnimation.Count - 1)
            {
                currentCombo = ComboAnimation.None;
            }
            SetCombo(currentCombo + 1);
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

        controller.player.animator.ResetTrigger("Attack");
    }
    
    private void SetCombo(ComboAnimation newCombo)
    {
        if (isFirst)
        {
            isFirst = false;
        }
        if (currentCombo == newCombo)
        {
            return;
        }
        controller.player.animator.SetTrigger("Attack");
        currentCombo = newCombo;
    }
}
