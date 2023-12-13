using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour, IRenewal
{ 
    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("장착 버튼")]
    public Button equipButton;

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
                    var table = CsvTableMgr.GetTable<WeaponTable>().dataTable[item.id];
                    //iconImage.sprite = ;
                    nameText.text = st[table.name];
                    statText.text = $"[공격력] {table.atk}\n[무기속성] {table.property}";
                    infoText.text = table.name.ToString(); // string table
                }
                
                break;

            case Equip.EquipType.Armor:
                {
                    var table = CsvTableMgr.GetTable<ArmorTable>().dataTable[item.id];
                    //iconImage.sprite = ;

                    nameText.text = st[table.name];
                    statText.text = $"[방어력] {table.defence}\n[부위] {table.type}";
                    infoText.text = $"[세트효과] {table.set_skill_id}";
                }

                break;
        }

        equipButton.interactable = !item.isEquip;
    }

    public void WearItem()
    {
        if (item == null)
        {
            return;
        }

        PlayDataManager.WearItem(item);
        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                InventoryManager.Instance.ShowWeapons(true);

                break;

            case Equip.EquipType.Armor:
                InventoryManager.Instance.ShowArmors(true);

                break;
        }

        Renewal();
    }
}
