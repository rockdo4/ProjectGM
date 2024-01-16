using UnityEngine;

public class EffectCinemachineNoise : EffectBase
{
    [Header("����")]
    [SerializeField] private float amplitude;

    [Header("������")]
    [SerializeField] private float frequency;

    [Header("�ǹ� ������")]
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
