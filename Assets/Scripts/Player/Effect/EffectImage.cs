using UnityEngine;
using UnityEngine.UIElements;

public class EffectImage : Effect
{

    public override void Init(Transform targetTransform = null)
    {
        gameObject.SetActive(false);
    }
}
