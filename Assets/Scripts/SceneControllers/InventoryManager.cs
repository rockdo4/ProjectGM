using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
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
    public GameObject sellPanel;
    private Equip[] sellList = new Equip[10]; // 최대 판매 개수
    private bool sellMode = false;

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
        },
        delegate (ItemButton button) // actionOnRelease
        {
            button.OnCountAct();
            button.iconImage.sprite = null;
            button.button.onClick.RemoveAllListeners();
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

        var weapons = PlayDataManager.data.WeaponInventory;
        foreach (var weapon in weapons)
        {
            Debug.Log(weapon.id);
            var go = buttonPool.Get();

            go.iconImage.sprite = weaponIconSO.GetSprite(weapon.id / 100 * 100);
            go.OnEquip(weapon.isEquip);

            go.button.onClick.AddListener(() => 
            {
                if (sellMode)
                {
                    //sellList.Add(weapon);

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

        var armors = PlayDataManager.data.ArmorInventory;
        foreach (var armor in armors)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = armorIconSO.GetSprite(armor.id);
            go.OnEquip(armor.isEquip);

            go.button.onClick.AddListener(() =>
            {
                if (sellMode)
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

    }

    public void ShowMaterials(bool isOn)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();

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

    public void SellMode()
    {
        if (!sellMode)
        {
            sellMode = true;

            sellPanel.SetActive(true);
        }
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
