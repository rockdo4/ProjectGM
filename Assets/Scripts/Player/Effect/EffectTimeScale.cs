using UnityEngine;

public class EffectTimeScale : EffectBase
{
    [Header("적용할 시간 배율")]
    [Range(0.1f, 3f)]
    public float timeScaleValue;
    private const float originalFixedDeltaTime = 0.02f;
    private float prevTimeScale;

    protected override void Update()
    {
        if (GameManager.instance.IsPaused)
        {
            return;
        }
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
        prevTimeScale = Time.timeScale;
        Time.timeScale = timeScaleValue;
        Time.fixedDeltaTime = Time.timeScale * originalFixedDeltaTime;
        base.PlayStart(direction);
    }
    public override void PlayEnd()
    {
        Time.timeScale = prevTimeScale;
        base.PlayEnd();
    }
}
