using UnityEngine;

public class EffectVibration : Effect
{
    public override void Init(Transform targetTransform = null)
    {

    }

    public override void PlayStart(Vector3 direction = default)
    {
        base.PlayStart(direction);
        Handheld.Vibrate();
    }
}
