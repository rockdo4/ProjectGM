using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentItemButton : MonoBehaviour, IRenewal
{
    [Header("아이템 분류")]
    public Equip.EquipType Type;

    [Header("방어구 분류"), Tooltip("아이템이 무기라면 None타입으로 설정할 것")]
    public Armor.ArmorType armorType;

    [Header("무기 IconSO")]
    public IconSO weaponIconSO;

    [Header("방어구 IconSO")]
    public IconSO armorIconSO;

    private Image iconImage;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
    }

    public void Renewal()
    {
        switch (Type)
        {
            case Equip.EquipType.Weapon:
                if (PlayDataManager.curWeapon == null)
                {
                    iconImage.sprite = null;

                    break;
                }
                iconImage.sprite = weaponIconSO.GetSprite(PlayDataManager.curWeapon.id / 100 * 100 + 1);
                // weapon icon level reset

                break;

            case Equip.EquipType.Armor:
                if (PlayDataManager.curArmor[armorType] == null)
                {
                    iconImage.sprite = null;

                    break;
                }
                iconImage.sprite = armorIconSO.GetSprite(PlayDataManager.curArmor[armorType].id / 100 * 100 + 1);
                break;

            default:
                Debug.LogWarning("Not Exist Item.ItemType! - CurrentItemButton");
                break;
        }
    }

    public void UnWear()
    {
        if (iconImage.sprite == null)
        {
            return;
        }
        PlayDataManager.UnWearItem(Type, armorType);
        Renewal();
    }
}
