using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour, IRenewal
{
    [Header("소지금 텍스트")]
    public TextMeshProUGUI moneyText;

    [Header("진동기능")]
    [SerializeField]
    private Toggle vibeToggle;

    public static TitleManager Instance;

    private void Awake()
    {
        Instance = this;

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        vibeToggle.isOn = PlayDataManager.data.Vibration;

        Renewal();
    }

    public void ClearData()
    {
        PlayDataManager.Reset();
        MyNotice.Instance.Notice("데이터를 초기화 하였습니다.");
        moneyText.text = PlayDataManager.data.Gold.ToString();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Renewal();
        }
    }

    public void Renewal()
    {
        moneyText.text = PlayDataManager.data.Gold.ToString();
    }

    public void OnVibe(bool value)
    {
        PlayDataManager.data.Vibration = value;
        PlayDataManager.Save();
    }
}
