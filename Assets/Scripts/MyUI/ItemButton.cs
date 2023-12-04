using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [Header("개수 텍스트")]
    [SerializeField]
    private TextMeshProUGUI countText;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("버튼")]
    public Button button;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void OnCountAct(bool isActive = false)
    {
        countText.gameObject.SetActive(isActive);
    }

    public void SetCount(int value)
    {
        countText.text = value.ToString();
    }
}