using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [Header("소지금 텍스트")]
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        moneyText.text = PlayDataManager.data.Gold.ToString();
    }

    public void ClearData()
    {
        PlayDataManager.Reset();
        MyNotice.Instance.Notice("데이터를 초기화 하였습니다.");
        moneyText.text = PlayDataManager.data.Gold.ToString();

    }
}
