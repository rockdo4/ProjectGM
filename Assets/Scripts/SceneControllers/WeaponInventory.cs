using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    public TextMeshProUGUI equipWeaponText;
    public TextMeshProUGUI equipArmorText;

    private Item equipWeapon = null;
    private Item equipArmor = null;

    public GameObject buttonPrefab;

    private void Start()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        TestAddItem();
        //UpdateUI();
    }

    private void TestAddItem()
    {
        if (PlayDataManager.data.Inventory.Count != 0)
        {
            UpdateUI();
            return;
        }

        StartCoroutine(tester());

    }

    private IEnumerator tester()
    {
        for (int i = 0; i < 5; i++)
        {
            var weapon = new Item(Item.ItemType.Weapon, UnityEngine.Random.Range((int)Item.WeaponID.Simple_Hammer, (int)Item.WeaponID.Gold_Spear));
            PlayDataManager.data.Inventory.Add(weapon);

            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 5; i++)
        {
            var armor = new Item(Item.ItemType.Armor, UnityEngine.Random.Range((int)Item.ArmorID.test1, (int)Item.ArmorID.test3));
            PlayDataManager.data.Inventory.Add(armor);

            yield return new WaitForEndOfFrame();
        }

        PlayDataManager.Save();

        UpdateUI();
    }

    public void UpdateUI()
    {
        ClearItemButton();

        foreach (var item in PlayDataManager.data.Inventory) 
        {
            switch (item.type)
            {
                case Item.ItemType.Weapon:
                    {
                        var go = Instantiate(buttonPrefab);
                        go.transform.SetParent(inventoryPanel.transform);

                        var text = go.GetComponentInChildren<TextMeshProUGUI>();
                        text.text = ((Item.WeaponID)item.id).ToString();

                        var button = go.GetComponent<Button>();
                        button.onClick.AddListener(() => 
                        {
                            EquipItem(item);
                        });

                        if (item.isEquip)
                        {
                            text.color = Color.red;
                            equipWeaponText.text = ((Item.WeaponID)item.id).ToString();
                            equipWeapon = item;
                        }
                    }
                    
                    break;

                case Item.ItemType.Armor:
                    {
                        var go = Instantiate(buttonPrefab);
                        go.transform.SetParent(inventoryPanel.transform);

                        var text = go.GetComponentInChildren<TextMeshProUGUI>();
                        text.text = ((Item.ArmorID)item.id).ToString();

                        var button = go.GetComponent<Button>();
                        button.onClick.AddListener(() => 
                        {
                            EquipItem(item);
                        });

                        if (item.isEquip)
                        {
                            text.color = Color.red;
                            equipArmorText.text = ((Item.ArmorID)item.id).ToString();
                            equipArmor = item;
                        }
                    }
                    break;

                default:
                    break;
            }
            
        }

    }

    public void ClearItemButton()
    {
        var arr = inventoryPanel.GetComponentsInChildren<Button>();
        if (arr != null)
        {
            foreach (var item in arr)
            {
                Destroy(item.gameObject);
            }
        }
        equipWeapon = null;
        equipWeaponText.text = "NULL";

        equipArmor = null;
        equipArmorText.text = "NULL";

    }

    public void EquipItem(Item item)
    {
        if (PlayDataManager.data.Equipment.TryGetValue(item.type, out DateTime value)) // 장착중인 장비가 있고
        {
            if (item.instanceID != value) // 동일하지 않은 객체일 때
            {
                // Inventory isEquip Unlock
                PlayDataManager.data.Inventory.Find(i => i.instanceID == value).isEquip = false;

                // Equipment Change
                PlayDataManager.data.Equipment[item.type] = item.instanceID;
                item.isEquip = true;
            }

        }
        else // 장착중인 장비가 없을 때
        {
            PlayDataManager.data.Equipment.Add(item.type, item.instanceID);
            item.isEquip = true;
        }

        PlayDataManager.Save();

        UpdateUI();
    }

    public void UnequipItem(int type)
    {
        switch ((Item.ItemType)type)
        {
            case Item.ItemType.Weapon:
                if (equipWeapon == null)
                {
                    return;
                }
                equipWeapon.isEquip = false;

                break;

            case Item.ItemType.Armor:
                if (equipArmor == null)
                {
                    return;
                }
                equipArmor.isEquip = false;

                break;

            default:
                break;
        }
        PlayDataManager.Save();

        UpdateUI();
    }
}
