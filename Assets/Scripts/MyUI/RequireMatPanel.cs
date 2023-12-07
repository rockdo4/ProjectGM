using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class RequireMatPanel : MonoBehaviour, IRenewal
{
    [Header("필요 아이템 텍스트")]
    public TextMeshProUGUI matText;

    [Header("요구량 텍스트")]
    public TextMeshProUGUI valueText;

    [Header("슬라이더")]
    public Slider slider;

    public void SetSlider(int count, int capacity)
    {
        valueText.text = $"{count} / {capacity}";
        slider.maxValue = capacity;
        slider.value = count;
    }

    public void Renewal()
    {
        gameObject.SetActive(true);
    }
}