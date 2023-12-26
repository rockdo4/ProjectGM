using UnityEngine;
using UnityEngine.Rendering;

public class EffectVignette : EffectBase
{
    private Volume volume;

    public override void Init(Transform targetTransform = null)
    {
        volume = Camera.main.GetComponent<Volume>();
        volume.enabled = false;
    }
    
    protected override void Update()
    {
        if (!IsPlay)
        {
            return;
        }
        base.Update();
        volume.weight = Mathf.Lerp(1, 0, Timer / duration);
    }

    public override void PlayStart(Vector3 direction = default)
    {
        base.PlayStart(direction);
        volume.enabled = true;
    }

    public override void PlayEnd()
    {
        volume.enabled = false;
        base.PlayEnd();
    }
}
