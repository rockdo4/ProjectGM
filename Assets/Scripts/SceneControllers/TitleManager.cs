using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Notice")]
    public TextMeshProUGUI noticeText;

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

    public void GoGame(string sceneName)
    {
        if (PlayDataManager.curWeapon == null)
        {
            Notice("무기를 먼저 장착해주십시오.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    private void Notice(string str)
    {
        noticeText.text = str;
        noticeText.gameObject.SetActive(true);
    }

    public void ClearData()
    {
        PlayDataManager.Reset();
        Notice("데이터를 초기화 하였습니다.");
        moneyText.text = PlayDataManager.data.Gold.ToString();

    }
}
