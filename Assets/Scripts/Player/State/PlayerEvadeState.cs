using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Vector3 direction;
    //private Vector3 startPosition;
    private const string triggerName = "Evade";

    private AnimationClip animationClip;
    private float animationLength;
    
    public PlayerEvadeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        //if (animationClip == null)
        //{
        //    animationClip = controller.player.Animator.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == triggerName);

        //    animationClip.AddEvent(new AnimationEvent()
        //    {
        //        time = animationClip.length,
        //        functionName = "EvadeEnd",
        //    });
        //}
        animationLength = 19f / 30f;
        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Wing);
        controller.player.evadeTimer = 0f;
        direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Up => Vector3.forward,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            _ => Vector3.zero
        };
        controller.player.Animator.SetFloat("X", direction.x);
        controller.player.Animator.SetFloat("Z", direction.z);
        controller.player.Animator.SetTrigger(triggerName);

        //startPosition = controller.player.Rigid.position;

        controller.player.effects.PlayEffect(EffectType.Evade, direction);
    }

    public override void Update()
    {
        controller.player.evadeTimer += Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        var rotation = controller.player.Rigid.rotation;
        rotation.x = 0f;
        float speed = controller.player.MoveDistance / animationLength;
        var force = rotation * direction * speed;
        controller.player.Rigid.AddForce(force, ForceMode.VelocityChange);
    }
    public override void Exit()
    {
    }

    private void EvadeEnd()
    {
        controller.SetState(PlayerController.State.Idle);
    }
}
