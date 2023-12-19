using TMPro;
using UnityEngine;

public class TitleManager : MonoBehaviour, IRenewal
{
    [Header("소지금 텍스트")]
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

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
}
