using UnityEngine;
using UnityEngine.UI;

public class UpgradeEquipButton : MonoBehaviour, IRenewal
{
    [Header("업그레이드 가능 이미지")]
    public Image upgradableImage;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("무기 IconSO")]
    public IconSO weaponIconSO;

    [Header("방어구 IconSO")]
    public IconSO armorIconSO;

    private Button button;
    private Equip item = null;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Renewal()
    {
        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                iconImage.sprite = weaponIconSO.GetSprite(item.id / 100 * 100);

                break;

            case Equip.EquipType.Armor:
                iconImage.sprite = armorIconSO.GetSprite(item.id);

                break;
        }

        upgradableImage.color = (IsUpgradable()) ? Color.blue : Color.gray;
    }

    private bool IsUpgradable()
    {
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        if (!ct.ContainsKey(item.id + 1))
        {
            Debug.Log($"{item.id} is Full Level");
            return false;
        }

        var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id + 1].lvup_module);
        if (mat == null)
        {
            Debug.Log("Not Exist Materials");
            return false;
        }

        if (mat.count < ct[item.id + 1].number_3)
        {
            Debug.Log("Lack Of Materials Count");
            return false;
        }

        if (PlayDataManager.data.Gold < ct[item.id + 1].gold)
        {
            Debug.Log("Lack Of Gold");
            return false;
        }
        // 인벤토리 공간 부족 (추후 추가 필요)


        return true;
    }

    public void SetEquip(Equip item)
    {
        this.item = item;
    }

    public void CreateMode(UpgradeManager um)
    {
        button.onClick.RemoveAllListeners();

        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                button.onClick.AddListener(() =>
                {
                    um.createWeaponPanel.SetEquip(item);
                    um.createWeaponPanel.iconImage.sprite = iconImage.sprite;
                    um.createWeaponPanel.Renewal();
                });
                break;

            case Equip.EquipType.Armor:
                button.onClick.AddListener(() =>
                {
                    um.createArmorPanel.SetEquip(item);
                    //um.createArmorPanel.iconImage.sprite = iconImage.sprite;
                    um.createArmorPanel.Renewal();
                });
                break;
        }
        

        iconImage.color = Color.black;
    }

    public void UpgradeMode(UpgradeManager um)
    {
        button.onClick.RemoveAllListeners();

        switch (item.type)
        {
            case Equip.EquipType.Weapon:
                button.onClick.AddListener(() => 
                {
                    um.upgradeWeaponPanel.SetEquip(item);
                    um.upgradeWeaponPanel.SetButton(this);
                    um.upgradeWeaponPanel.beforeIconImage.sprite = iconImage.sprite;
                    um.upgradeWeaponPanel.afterIconImage.sprite = iconImage.sprite;
                    um.upgradeWeaponPanel.Renewal();
                });
                break;

            case Equip.EquipType.Armor:
                button.onClick.AddListener(() => 
                {

                });
                break;
        }

        iconImage.color = Color.white;
    }
}