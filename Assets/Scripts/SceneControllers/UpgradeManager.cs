using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("현재 장비 버튼들")]
    public List<CurrentItemButton> equipButtons;

    [Header("플레이어 정보")]
    public UpgradeInfoPanel upgradeInfoPanel;

    [Header("무기/방어구 제작 패널")]
    public CreateWeaponPanel createWeaponPanel;
    public CreateArmorPanel createArmorPanel;

    [Header("무기/방어구 업그레이드 패널")]
    public UpgradeEquipPanel upgradeWeaponPanel;
    public UpgradeEquipPanel upgradeArmorPanel;

    [Space(10.0f)]

    public UpgradeEquipButton buttonPrefab;

    public GameObject content;

    private ObjectPool<UpgradeEquipButton> buttonPool;
    private List<UpgradeEquipButton> releaseList = new List<UpgradeEquipButton>();

    private void Awake()
    {
        Instance = this;

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        
    }

    private void Start()
    {
        buttonPool = new ObjectPool<UpgradeEquipButton>(
            () => // createFunc
            {
                var button = Instantiate(buttonPrefab);
                button.transform.SetParent(content.transform);
                button.gameObject.SetActive(false);

                return button;
            },
        delegate (UpgradeEquipButton button) // actionOnGet
        {
            button.gameObject.SetActive(true);
            button.transform.SetParent(content.transform);
        },
        delegate (UpgradeEquipButton button) // actionOnRelease
        {
            button.iconImage.sprite = null;
            button.transform.SetParent(transform);
            button.gameObject.SetActive(false);
        });

        foreach (var renewal in equipButtons)
        {
            renewal.Renewal();
        }
        upgradeInfoPanel.Renewal();

        ShowWeapons(true);
    }

    public void ShowWeapons(bool isOn = true)
    {
        if (!isOn)
        {
            return;
        }
        Clear();

        /*
        var inv = PlayDataManager.data.WeaponInventory;
        foreach (var item in inv)
        {
            var go = buttonPool.Get();

            go.SetEquip(item);
            go.UpgradeMode(this);
            go.Renewal();

            releaseList.Add(go);
        }
        */

        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        foreach (var data in ct)
        {
            if (data.Value._class != Equip.EquipType.Weapon)
            {
                continue;
            }

            if (data.Value.mf_module != -1)
            {
                var item = new Weapon(data.Key);
                var go = buttonPool.Get();

                go.SetEquip(item);
                go.CreateMode(this);
                go.Renewal();

                releaseList.Add(go);
            }
            
        }
    }

    public void ShowArmors(bool isOn = true)
    {
        if (!isOn)
        {
            return;
        }
        Clear();

        /*
        var inv = PlayDataManager.data.ArmorInventory;
        foreach (var item in inv)
        {
            var go = buttonPool.Get();

            go.SetEquip(item);
            go.UpgradeMode(this);
            go.Renewal();

            releaseList.Add(go);
        }
        */
        
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        foreach (var data in ct)
        {
            if (data.Value._class != Equip.EquipType.Armor)
            {
                continue;
            }

            if (data.Value.mf_module != -1)
            {
                var item = new Armor(data.Key);
                var go = buttonPool.Get();

                go.SetEquip(item);
                go.CreateMode(this);
                go.Renewal();

                releaseList.Add(go);
            }

        }
        
    }

    public void Clear()
    {
        foreach (var item in releaseList)
        {
            buttonPool.Release(item);
        }

        releaseList.Clear();
    }
}
