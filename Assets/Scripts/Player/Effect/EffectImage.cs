using UnityEngine;
using UnityEngine.UI;

public class EffectImage : Effect
{
    [Header("페이드 아웃 사용 여부")]
    [SerializeField]
    private bool UseFadeOut = false;
    [Header("페이드 아웃 대기 시간")]
    [SerializeField]
    private float startTime = 0f;

    private Image image;
    private Color originalColor;

    public override void Init(Transform targetTransform = null)
    {
        image = prefab.GetComponent<Image>();
        originalColor = image.color;
        gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if (!UseFadeOut || Timer < startTime)
        {
            return;
        }

        var color = image.color;
        color.a = Mathf.Lerp(1, 0, Timer / duration);
        image.color = color;
    }

    public override void PlayEnd()
    {
        image.color = originalColor;
        base.PlayEnd();
    }
}
