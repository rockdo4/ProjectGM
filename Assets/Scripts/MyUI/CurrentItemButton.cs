using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentItemButton : MonoBehaviour, IRenewal
{
    [Header("아이템 분류")]
    public Equip.EquipType Type;

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
            case Equip.EquipType.Weapon:
                if (PlayDataManager.curWeapon == null)
                {
                    tester.text = "X";

                    break;
                }
                tester.text = PlayDataManager.curWeapon.id.ToString();
                break;

            case Equip.EquipType.Armor:
                if (PlayDataManager.curArmor[armorType] == null)
                {
                    tester.text = "X";

                    break;
                }
                tester.text = PlayDataManager.curArmor[armorType].id.ToString();
                break;

            default:
                Debug.LogWarning("Not Exist Item.ItemType! - CurrentItemButton");
                break;
        }
    }

    public void UnWear()
    {
        PlayDataManager.UnWearItem(Type, armorType);
        Renewal();
    }
}
