using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class InventoryManager : MonoBehaviour
{
    public enum ItemType
    {
        Weapon,
        Armor,
        SkillCode,
        Mat
    }
    private ItemType curType = ItemType.Weapon;
    public static InventoryManager Instance;

    public GameObject inventoryPanel;

    public ItemButton buttonPrefab;

    [Header("무기/방어구")]
    public ItemPanel itemPanel;
    public IconSO weaponIconSO;
    public IconSO armorIconSO;

    [Space(10.0f)]

    [Header("장식주")]
    public GameObject decoPanel;

    [Space(10.0f)]

    [Header("재료")]
    public MatPanel matPanel;
    public IconSO matIconSo;

    [Space(10.0f)]

    [Header("일괄판매")]
    public GameObject sellArea;
    public GameObject sellPanel;
    public GameObject sellButton;

    private bool sellMode = false;
    private List<Equip> sellEquipList = new List<Equip>();

    private ObjectPool<ItemButton> buttonPool;
    private List<ItemButton> releaseList = new List<ItemButton>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buttonPool = new ObjectPool<ItemButton>(
            () => // createFunc
        {
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(inventoryPanel.transform);
            button.OnCountAct();
            button.gameObject.SetActive(false);

            return button;
        },
        delegate (ItemButton button) // actionOnGet
        {
            button.gameObject.SetActive(true);
            button.transform.SetParent(inventoryPanel.transform);
        },
        delegate (ItemButton button) // actionOnRelease
        {
            button.OnCountAct();
            button.iconImage.sprite = null;
            button.iconImage.color = Color.white;
            button.button.onClick.RemoveAllListeners();
            button.transform.SetParent(gameObject.transform); // ItemButton Transform Reset
            button.gameObject.SetActive(false);
        });

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        //TestAddItem();
        ShowWeapons(true);
    }

    public void ShowWeapons(bool isOn)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.Weapon;

        var weapons = PlayDataManager.data.WeaponInventory;
        foreach (var weapon in weapons)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = weaponIconSO.GetSprite(weapon.id / 100 * 100);
            go.iconImage.color = Color.white;
            go.OnEquip(weapon.isEquip);

            go.button.onClick.AddListener(() => 
            {
                if (sellMode && sellEquipList.Count <= 10)
                {
                    if (go.iconImage.color == Color.white)
                    {
                        sellEquipList.Add(weapon);
                        go.iconImage.color = Color.red;

                        var newGo = buttonPool.Get();
                        newGo.iconImage.sprite = weaponIconSO.GetSprite(weapon.id / 100 * 100);
                        newGo.OnEquip(weapon.isEquip);
                        newGo.transform.SetParent(sellPanel.transform);
                        newGo.button.onClick.AddListener(() => 
                        {
                            buttonPool.Release(go.sell);
                            go.sell = null;
                            sellEquipList.Remove(weapon);
                            go.iconImage.color = Color.white;
                        });

                        go.sell = newGo;
                    }
                    else
                    {
                        buttonPool.Release(go.sell);
                        go.sell = null;
                        sellEquipList.Remove(weapon);
                        go.iconImage.color = Color.white;
                    }
                }
                else
                {
                    itemPanel.SetItem(weapon);
                    itemPanel.iconImage.sprite = go.iconImage.sprite;
                    itemPanel.Renewal();
                }
                
            });

            releaseList.Add(go);
        }
    }

    public void ShowArmors(bool isOn)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.Armor;

        var armors = PlayDataManager.data.ArmorInventory;
        foreach (var armor in armors)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = armorIconSO.GetSprite(armor.id);
            go.iconImage.color = Color.white;
            go.OnEquip(armor.isEquip);

            go.button.onClick.AddListener(() =>
            {
                if (sellMode && sellEquipList.Count <= 10)
                {

                }
                else
                {
                    itemPanel.SetItem(armor);
                    itemPanel.iconImage.sprite = go.iconImage.sprite;
                    itemPanel.Renewal();
                }
                
            });

            releaseList.Add(go);
        }
    }

    public void ShowDecorations(bool isOn)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.SkillCode;
    }

    public void ShowMaterials(bool isOn)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.Mat;

        var mats = PlayDataManager.data.MatInventory;
        foreach (var mat in mats)
        {
            var go = buttonPool.Get();
            go.transform.SetParent(inventoryPanel.transform);

            go.OnCountAct(true, mat.count);
            go.iconImage.sprite = matIconSo.GetSprite(mat.id);
            go.OnEquip();

            go.GetComponent<ItemButton>().button.onClick.AddListener(() =>
            {
                // if (sellMode)
                matPanel.SetMaterials(mat);
                matPanel.iconImage.sprite = go.iconImage.sprite;
                matPanel.Renewal();
            });

            releaseList.Add(go);
        }
    }

    public void ClearItemButton()
    {
        foreach (var item in releaseList)
        {
            buttonPool.Release(item);
        }

        releaseList.Clear();
    }

    public void SellMode(bool mode)
    {
        sellMode = mode;
        sellArea.SetActive(mode);
        sellButton.SetActive(!mode);

        switch (curType)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
                sellEquipList.Clear();
                break;

            case ItemType.SkillCode:
                // 구현 필요
                break;

            case ItemType.Mat:
                // 구현 필요
                break;
        }

        switch (sellMode)
        {
            case true:

                break;

            case false:
                foreach (var item in releaseList)
                {
                    if (item.sell != null)
                    {
                        buttonPool.Release(item.sell);
                        item.sell = null;

                    }
                    item.iconImage.color = Color.white;
                }
                break;
        }
    }

    public void SellItem()
    {
        switch (curType)
        {
            case ItemType.Weapon:
                {
                    foreach (var item in sellEquipList)
                    {
                        PlayDataManager.SellItem(item as Weapon);
                    }
                    ShowWeapons(true);
                }
                break;

            case ItemType.Armor:
                {
                    foreach (var item in sellEquipList)
                    {
                        PlayDataManager.SellItem(item as Armor);
                    }
                    ShowArmors(true);
                }
                break;

            case ItemType.SkillCode:
                {
                    // 구현 필요
                }
                break;

            case ItemType.Mat:
                {
                    // 구현 필요
                }
                break;
        }
        SellMode(false);

    }

    public void Tester()
    {
        PlayDataManager.data.MatInventory.Add(new Materials(71001, 99));
        PlayDataManager.data.MatInventory.Add(new Materials(72001, 99));
        PlayDataManager.data.MatInventory.Add(new Materials(73001, 99));
        PlayDataManager.data.MatInventory.Add(new Materials(73002, 99));
        PlayDataManager.data.MatInventory.Add(new Materials(73003, 99));
        PlayDataManager.data.MatInventory.Add(new Materials(73004, 99));
        PlayDataManager.data.MatInventory.Add(new Materials(73005, 99));

        PlayDataManager.AddGold(100000);

        StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        {
            var armor = new Armor(100001);
            PlayDataManager.data.ArmorInventory.Add(armor);
            yield return new WaitForEndOfFrame();
        }

        {
            var armor = new Armor(100002);
            PlayDataManager.data.ArmorInventory.Add(armor);
            yield return new WaitForEndOfFrame();
        }

        {
            var armor = new Armor(100003);
            PlayDataManager.data.ArmorInventory.Add(armor);
            yield return new WaitForEndOfFrame();
        }

        {
            var armor = new Armor(100004);
            PlayDataManager.data.ArmorInventory.Add(armor);
            yield return new WaitForEndOfFrame();
        }

        {
            var armor = new Armor(100005);
            PlayDataManager.data.ArmorInventory.Add(armor);
            yield return new WaitForEndOfFrame();
        }
    }
}
