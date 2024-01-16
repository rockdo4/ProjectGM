using UnityEngine;

public class EffectCinemachineNoise : EffectBase
{
    [Header("진폭")]
    [SerializeField] private float amplitude;

    [Header("진동수")]
    [SerializeField] private float frequency;

    [Header("피벗 오프셋")]
    [SerializeField] private Vector3 pivotOffset;

    public override void PlayStart(Vector3 direction = default)
    {
        base.PlayStart(direction);
        CameraManager.Instance.Noise(amplitude, frequency, pivotOffset);
    }

    public override void PlayEnd()
    {
        CameraManager.Instance.StopNoise();
        base.PlayEnd();
    }

    public override void Init(Transform targetTransform = null)
    {
        
    }
}
