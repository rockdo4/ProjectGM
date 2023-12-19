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
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI additionalText;

    [Header("업그레이드 패널")]
    public UpgradeEquipPanel upgradePanel;

    public Equip item = null;

    public void Renewal()
    {
        gameObject.SetActive(true);

        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                {
                    var table = CsvTableMgr.GetTable<WeaponTable>().dataTable[item.id];
                    //iconImage.sprite = ;
                    nameText.text = st[table.name];
                    valueText.text = table.atk.ToString();
                    additionalText.text = table.property.ToString();
                }

                break;

            case Equip.EquipType.Armor:
                {
                    var table = CsvTableMgr.GetTable<ArmorTable>().dataTable[item.id];
                    //iconImage.sprite = ;

                    nameText.text = st[table.name];
                    valueText.text = table.defence.ToString();
                    additionalText.text = $"[세트효과] {table.set_skill_id}";
                }

                break;
        }

        equipButton.gameObject.SetActive(!item.isEquip);

        /*
        if (item.isEquip)
        {
            valueText.color = Color.red;
        }
        else
        {
            valueText.color = Color.white;
        }
        */
    }

    public void SetItem(Equip item)
    {
        this.item = item;
        
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

    public void UpgradeItem()
    {
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        if (!ct.ContainsKey(item.id + 1))
        {
            MyNotice.Instance.Notice("강화를 진행할 수 없습니다.");
            return;
        }

        upgradePanel.SetEquip(item);
        upgradePanel.SetIconImage(iconImage.sprite);
        upgradePanel.SetItemPanel(this);
        upgradePanel.Renewal();
    }
}
