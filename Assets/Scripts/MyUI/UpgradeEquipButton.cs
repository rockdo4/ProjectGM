using Unity.VisualScripting;
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
        iconImage.sprite = item.type switch
        {
            Equip.EquipType.Weapon => weaponIconSO.GetSprite(item.id / 100 * 100 + 1),
            Equip.EquipType.Armor => armorIconSO.GetSprite(item.id / 100 * 100 + 1)
        };

        upgradableImage.color = (IsUpgradable()) ? Color.blue : Color.gray;
    }

    private bool IsUpgradable()
    {
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        if (!ct.ContainsKey(item.id + 1))
        {
            //Debug.Log($"{item.id} is Full Level");
            return false;
        }

        var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id + 1].lvup_module);
        if (mat == null)
        {
            //Debug.Log("Not Exist Materials");
            return false;
        }

        if (mat.count < ct[item.id + 1].lvup_module_req)
        {
            //Debug.Log("Lack Of Materials Count");
            return false;
        }

        if (PlayDataManager.data.Gold < ct[item.id + 1].gold)
        {
            //Debug.Log("Lack Of Gold");
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
                    um.createArmorPanel.iconImage.sprite = iconImage.sprite;
                    um.createArmorPanel.Renewal();
                });
                break;
        }
        

        //iconImage.color = Color.black;
    }

    /*
    public void UpgradeMode(UpgradeManager um)
    {
        button.onClick.RemoveAllListeners();

        var umPanel = item.type switch
        {
            Equip.EquipType.Weapon => um.upgradeWeaponPanel,
            Equip.EquipType.Armor => um.upgradeArmorPanel
        };

        button.onClick.AddListener(() =>
        {
            var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
            if (!ct.ContainsKey(item.id + 1))
            {
                MyNotice.Instance.Notice("강화를 진행할 수 없습니다.");
                return;
            }
            umPanel.SetEquip(item);
            umPanel.SetButton(this);
            umPanel.beforeIconImage.sprite = iconImage.sprite;
            umPanel.afterIconImage.sprite = iconImage.sprite;
            umPanel.Renewal();
        });

        iconImage.color = Color.white;
    }
    */

    private void OnEnable()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        transform.localScale = Vector3.one;
#endif
    }
}