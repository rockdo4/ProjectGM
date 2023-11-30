using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentItemButton : MonoBehaviour, IRenewal
{
    [Header("아이템 분류")]
    public Item.ItemType Type;

    [Header("방어구 분류")]
    public Armor.ArmorType armorType;

    private Image iconImage;
    private TextMeshProUGUI tester; // test code

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        tester = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Renewal()
    {
        switch (Type)
        {
            case Item.ItemType.Weapon:
                if (PlayDataManager.curWeapon == null)
                {
                    break;
                }
                tester.text = PlayDataManager.curWeapon.id.ToString();
                break;

            case Item.ItemType.Armor:
                if (!PlayDataManager.curArmor.ContainsKey(armorType))
                {
                    break;
                }
                tester.text = PlayDataManager.curArmor[armorType].id.ToString();
                break;

            default:
                Debug.LogWarning("Not Exist Item.ItemType! - CurrentItemButton");
                break;
        }
    }
}
