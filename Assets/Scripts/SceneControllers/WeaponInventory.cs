using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    public GameObject inventoryPanel;

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
            return;
        }

        StartCoroutine(tester());

    }

    private IEnumerator tester()
    {
        for (int i = 0; i < 5; i++)
        {
            var weapon = new Item(Item.ItemType.Weapon, (int)Item.WeaponID.Simple_Hammer);
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
                        var go = new GameObject(item.id.ToString());
                        go.transform.SetParent(inventoryPanel.transform);

                        var text = go.AddComponent<TextMeshProUGUI>();
                        text.fontSize = 15;
                        text.text = ((Item.WeaponID)item.id).ToString();

                        if (item.isEquip)
                        {
                            text.color = Color.red;

                        }
                    }
                    
                    break;

                case Item.ItemType.Armor:
                    {
                        var go = new GameObject(item.id.ToString());
                        go.transform.SetParent(inventoryPanel.transform);

                        var text = go.AddComponent<TextMeshProUGUI>();
                        text.fontSize = 15;

                        text.text = ((Item.ArmorID)item.id).ToString();

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
}
