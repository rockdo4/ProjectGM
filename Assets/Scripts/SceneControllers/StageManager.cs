using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
    }

    public void GoGame(string sceneName)
    {
        if (PlayDataManager.curWeapon == null)
        {
            MyNotice.Instance.Notice("무기를 먼저 장착해주십시오.");
            return;
        }
        BLACK.FadeOut(sceneName);
    }
}
