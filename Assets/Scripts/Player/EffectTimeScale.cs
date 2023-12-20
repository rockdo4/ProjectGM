using UnityEngine;

public class EffectTimeScale : Effect
{
    [Header("적용할 시간 배율")]
    [Range(0.1f, 3f)]
    public float timeScaleValue;
    private const float originalFixedDeltaTime = 0.02f;

    protected override void Update()
    {
        base.Update();
        Time.timeScale += (1f / duration) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = Time.timeScale * originalFixedDeltaTime;
    }

    public override void Init(Transform targetTransform = null)
    {
    }

    public override void PlayStart(Vector3 direction = default)
    {
        Time.timeScale = timeScaleValue;
        Time.fixedDeltaTime = Time.timeScale * originalFixedDeltaTime;
        base.PlayStart(direction);
    }
}
