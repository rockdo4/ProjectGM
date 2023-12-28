using UnityEngine;
using Lofelt.NiceVibrations;
public class EffectVibration : EffectBase
{
    [Header("진폭")]
    [SerializeField] private float amplitude;

    [Header("진동수")]
    [SerializeField] private float frequency;

    public override void Init(Transform targetTransform = null)
    {

    }

    public override void PlayStart(Vector3 direction = default)
    {
        if (!PlayDataManager.data.Vibration)
        {
            return;
        }
        base.PlayStart(direction);
        HapticPatterns.PlayConstant(amplitude, frequency, duration);
    }

    public override void PlayEnd()
    {
        HapticController.Stop();
        base.PlayEnd();
    }
}
