using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [Header("개수 텍스트")]
    [SerializeField]
    private TextMeshProUGUI countText;

    [Header("장착중 이미지")]
    [SerializeField]
    private Image equipImage;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("버튼")]
    public Button button;

    public ItemButton sell = null;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void OnCountAct(bool isActive = false, int value = 0)
    {
        countText.gameObject.SetActive(isActive);
        countText.text = value.ToString();

    }

    public void OnEquip(bool isEquip = false)
    {
        equipImage.gameObject.SetActive(isEquip);
    }

    private void OnEnable()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        transform.localScale = Vector3.one;
#endif
    }
    
}