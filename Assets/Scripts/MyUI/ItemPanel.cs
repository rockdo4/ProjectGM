using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour, IRenewal
{ 
    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("텍스트 모음")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statText;
    public TextMeshProUGUI infoText;

    public Equip item = null;

    public void Renewal()
    {
        gameObject.SetActive(true);

        if (item.isEquip)
        {
            statText.color = Color.red;
        }
        else
        {
            statText.color = Color.white;
        }
    }

    public void SetItem(Equip item)
    {
        this.item = item;

        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                {
                    var table = CsvTableMgr.GetTable<WeaponTable>().dataTable[(Weapon.WeaponID)item.id];
                    //iconImage.sprite = ;
                    nameText.text = ((Weapon.WeaponID)table.weapon_name).ToString(); // string table
                    statText.text = $"[공격력] {table.atk}\n[무기속성] {table.property}";
                    infoText.text = table.weapon_name.ToString(); // string table
                }
                
                break;

            case Equip.EquipType.Armor:
                {
                    //iconImage.sprite = ;

                }

                break;
        }
    }

    public void WearItem()
    {
        if (item == null)
        {
            return;
        }

        PlayDataManager.WearItem(item);

        Renewal();
    }
}
