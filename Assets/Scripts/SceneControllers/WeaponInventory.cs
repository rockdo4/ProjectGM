using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    public GameObject inventoryPanel;
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
        PlayDataManager.Save();

        UpdateUI();
    }

    public void UpdateUI()
    {
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
                        }
                    }
                    break;

                default:
                    break;
            }
            
        }

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
    }
}
