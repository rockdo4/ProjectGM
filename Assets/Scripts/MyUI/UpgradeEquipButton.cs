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


        return false;
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