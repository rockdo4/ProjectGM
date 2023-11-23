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
        Player2.Instance.evadeTimer = 0f;
        direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Up => Vector3.forward,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            _ => Vector3.forward
        };

        Player2.Instance.anim.SetFloat("X", direction.x);
        Player2.Instance.anim.SetFloat("Z", direction.z);
        Player2.Instance.anim.SetTrigger("Evade");

        startPosition = Player2.Instance.rigid.position;
    }

    public override void Update()
    {
        Player2.Instance.evadeTimer += Time.deltaTime;

        if (Player2.Instance.evadeTimer > Player2.Instance.stat.evadeTime)
        {
            Player2.Instance.SetState(Player2.States.Idle);
        }
    }

    public override void FixedUpdate()
    {
        var position = Player2.Instance.rigid.position;
        if (Vector3.Distance(startPosition, position) < Player2.Instance.EvadeDistance)
        {
            var rotation = Player2.Instance.rigid.rotation;
            var moveSpeed = Player2.Instance.stat.MoveSpeed;
            Player2.Instance.rigid.MovePosition(position + rotation * direction * moveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
    }
}
