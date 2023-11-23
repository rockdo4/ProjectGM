using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Vector3 direction;
    private Vector3 startPosition;
    
    private float EvadeDistance
    {
        get
        {
            return Player2.Instance.colldier.bounds.size.y * 2;
        }
    }

    public PlayerEvadeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        Player2.Instance.evadeTimer = 0f;
        direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Up => Vector3.forward,
            _ => Vector3.zero
        };

        Player2.Instance.ren.material.color = Player2.Instance.evadeColor;
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
        if (Vector3.Distance(startPosition, position) < EvadeDistance)
        {
            var rotation = Player2.Instance.rigid.rotation;
            var moveSpeed = Player2.Instance.stat.MoveSpeed;
            Player2.Instance.rigid.MovePosition(position + rotation * direction * moveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        Player2.Instance.ren.material.color = Player2.Instance.originalColor;
    }
}
