using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWeaponPanel : MonoBehaviour, IRenewal
{
    [Header("이름 텍스트")]
    public TextMeshProUGUI beforeNameText;
    public TextMeshProUGUI afterNameText;

    [Header("아이콘 이미지")]
    public Image beforeIconImage;
    public Image afterIconImage;

    [Header("설명 텍스트")]
    public TextMeshProUGUI beforeInfoText;
    public TextMeshProUGUI afterInfoText;

    [Header("요구 재료 패널")]
    public RequireMatPanel require;

    [Header("요구 재료 패널 영역")]
    public GameObject content;

    [Header("요구 금액 텍스트")]
    public TextMeshProUGUI priceText;

    private UpgradeEquipButton button = null;
    private Equip item = null;

    public void SetEquip(Equip item)
    {
        this.item = item;
    }

    public void SetButton(UpgradeEquipButton button) 
    { 
        this.button = button;
    }

    public void Renewal()
    {
        gameObject.SetActive(true);

        var cons = content.GetComponentsInChildren<RequireMatPanel>();
        foreach (var c in cons)
        {
            Destroy(c.gameObject);
        }

        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        var wt = CsvTableMgr.GetTable<WeaponTable>().dataTable;
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        var mt = CsvTableMgr.GetTable<MatTable>().dataTable;

        if (!ct.ContainsKey(item.id + 1))
        {
            gameObject.SetActive(false);
            return;
        }

        beforeNameText.text = st[wt[item.id].name];
        afterNameText.text = st[wt[item.id + 1].name];

        beforeInfoText.text = $"현재 공격력 : {wt[item.id].atk}\n현재 속성 : {wt[item.id].weakpoint}";
        afterInfoText.text = $"현재 공격력 : {wt[item.id + 1].atk}\n현재 속성 : {wt[item.id + 1].weakpoint}";

        if (ct[item.id + 1].lvup_module != -1) // 요구 재료마다 분기
        {
            var go = Instantiate(require, content.transform);
            var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id + 1].lvup_module);
            var count = 0;
            if (mat != null)
            {
                count = mat.count;
            }
            go.matText.text = st[mt[ct[item.id + 1].lvup_module].name];
            go.SetSlider(count, ct[item.id + 1].lvup_module_req);
            go.Renewal();
        }

        if (ct[item.id + 1].ingredients != -1) // 요구 재료마다 분기
        {
            var go = Instantiate(require, content.transform);
            go.matText.text = st[wt[ct[item.id + 1].ingredients].name];
            go.SetSlider(1, 1);
            go.Renewal();
        }

        priceText.text = $"비용 : {ct[item.id + 1].gold}\n소지금 : {PlayDataManager.data.Gold}";
    }

    public void UpgradeEquip()
    {
        if (!IsUpgradable())
        {
            return;
        }

        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;

        var weapon = new Weapon(item.id + 1);
        PlayDataManager.Purchase(ct[item.id + 1].gold);
        PlayDataManager.DecreaseMat(ct[item.id + 1].lvup_module, ct[item.id + 1].lvup_module_req);
        if (item.isEquip)
        {
            PlayDataManager.WearItem(weapon);
        }
        PlayDataManager.data.WeaponInventory.Remove(item as Weapon);
        PlayDataManager.data.WeaponInventory.Add(weapon);
        PlayDataManager.Save();

        item = weapon;
        button.SetEquip(item);
        Renewal();
    }

    private bool IsUpgradable()
    {
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;

        var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id + 1].lvup_module);
        if (mat == null)
        {
            Debug.Log("Not Exist Materials");
            return false;
        }

        if (mat.count < ct[item.id + 1].lvup_module_req)
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
}