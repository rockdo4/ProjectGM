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
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                {
                    var table = CsvTableMgr.GetTable<WeaponTable>().dataTable[(Weapon.WeaponID)item.id];
                    //iconImage.sprite = ;
                    nameText.text = st[table.weapon_name];
                    statText.text = $"[공격력] {table.atk}\n[무기속성] {table.property}";
                    infoText.text = table.weapon_name.ToString(); // string table
                }
                
                break;

            case Equip.EquipType.Armor:
                {
                    var table = CsvTableMgr.GetTable<ArmorTable>().dataTable[(Armor.ArmorID)item.id];
                    //iconImage.sprite = ;

                    nameText.text = st[table.Armor_name];
                    statText.text = $"[방어력] {table.def}\n[부위] {table.Armor_type}";
                    infoText.text = $"[세트효과] {table.set_skill}";
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
