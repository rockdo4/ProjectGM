using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("현재 장비 버튼들")]
    public List<CurrentItemButton> renewals;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        
    }

    private void Start()
    {
        foreach (var renewal in renewals)
        {
            renewal.Renewal();
        }
    }
}
