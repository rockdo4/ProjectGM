using UnityEngine;
using UnityEngine.Rendering;

public class EffectVignette : Effect
{
    private Camera cam;

    public override void Init(Transform targetTransform = null)
    {
        cam.GetComponent<Volume>();
    }

    public override void PlayStart(Vector3 direction = default)
    {
    }

    public override void PlayEnd()
    {

    }
}
