using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Vector3 direction;
    private Vector3 startPosition;

    public PlayerEvadeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        Player.Instance.evadeTimer = 0f;
        direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Up => Vector3.forward,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            _ => Vector3.zero
        };

        Player.Instance.anim.SetFloat("X", direction.x);
        Player.Instance.anim.SetFloat("Z", direction.z);
        Player.Instance.anim.SetTrigger("Evade");

        startPosition = Player.Instance.rigid.position;
    }

    public override void Update()
    {
        Player.Instance.evadeTimer += Time.deltaTime;

        if (Player.Instance.evadeTimer > Player.Instance.stat.evadeTime)
        {
            Player.Instance.SetState(Player.States.Idle);
        }
    }

    public override void FixedUpdate()
    {
        var position = Player.Instance.rigid.position;
        if (Vector3.Distance(startPosition, position) < Player.Instance.EvadeDistance)
        {
            var rotation = Player.Instance.rigid.rotation;
            var moveSpeed = Player.Instance.stat.MoveSpeed;
            Player.Instance.rigid.MovePosition(position + rotation * direction * moveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
    }
}
