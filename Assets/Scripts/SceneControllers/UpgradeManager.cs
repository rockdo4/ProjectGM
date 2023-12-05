using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("현재 장비 버튼들")]
    public List<CurrentItemButton> equipButtons;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        
    }

    private void Start()
    {
        foreach (var renewal in equipButtons)
        {
            renewal.Renewal();
        }

        ShowWeapons(true);
    }

    public void ShowWeapons(bool isOn)
    {
        if (!isOn)
        {
            Clear();
            return;
        }
        Debug.Log("W");
    }

    public void ShowArmors(bool isOn)
    {
        if (!isOn)
        {
            Clear();
            return;
        }
        Debug.Log("A");

    }

    public void Clear()
    {
        Debug.Log("Clear");
    }
}
