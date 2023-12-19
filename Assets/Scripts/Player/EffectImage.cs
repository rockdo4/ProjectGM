using UnityEngine;
using UnityEngine.UIElements;

public class EffectImage : Effect
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Init(Transform transform = null)
    {
        
    }
}
