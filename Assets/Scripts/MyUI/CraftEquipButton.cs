using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftEquipButton : MonoBehaviour, IRenewal
{
    [Header("제작 가능 이미지")]
    public Image upImage;

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
            Equip.EquipType.Armor => armorIconSO.GetSprite(item.id / 100 * 100 + 1),

            _ => null
        };

        upImage.color = (IsUpgradable()) ? Color.blue : Color.gray;
    }

    private bool IsUpgradable()
    {
        /*
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
        */
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;

        var mat1 = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id].mf_module);
        if (mat1 == null)
        {
            //Debug.Log("Not Exist Materials");
            return false;
        }

        if (mat1.count < ct[item.id].mf_module_req)
        {
            //Debug.Log("Lack Of Materials Count");
            return false;
        }

        if (ct[item.id].mon_core != -1)
        {
            var mat2 = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id].mon_core);
            if (mat2 == null)
            {
                //Debug.Log("Not Exist Materials");
                return false;
            }

            if (mat2.count < ct[item.id].mon_core_req)
            {
                //Debug.Log("Lack Of Materials Count");
                return false;
            }
        }

        if (PlayDataManager.data.Gold < ct[item.id].gold)
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

    public void CreateMode(CraftManager um)
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