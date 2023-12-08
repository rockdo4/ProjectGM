using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public TextMeshProUGUI noticeText;

    public void GoGame(string sceneName)
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        
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
}
