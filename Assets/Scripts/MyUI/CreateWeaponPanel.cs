using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateWeaponPanel : MonoBehaviour, IRenewal
{
    [Header("무기 이름 텍스트")]
    [SerializeField]
    private TextMeshProUGUI nameText;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("공격력 텍스트")]
    [SerializeField]
    private TextMeshProUGUI atkText;

    [Header("공격 속성 텍스트")]
    [SerializeField] 
    private TextMeshProUGUI attackTypeText;

    [Header("요구 재료 패널")]
    [SerializeField]
    private RequireMatPanel require;

    [Header("요구 재료 패널 영역")]
    [SerializeField]
    private GameObject content;

    [Header("요구 금액 텍스트")]
    [SerializeField] 
    private TextMeshProUGUI priceText;

    [Header("장착하기 버튼")]
    [SerializeField]
    private Button craftButton;

    [Header("제작 파티클")]
    [SerializeField]
    private ParticleSystem craftParticle;

    private Equip item = null;

    public void SetEquip(Equip item)
    {
        this.item = item;
    }

    public void Renewal()
    {
        gameObject.SetActive(true);

        var cons = content.GetComponentsInChildren<RequireMatPanel>();
        foreach (var c in cons)
        {
            Destroy(c.gameObject);
        }

        var weapon = CsvTableMgr.GetTable<WeaponTable>().dataTable[item.id];
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        var mt = CsvTableMgr.GetTable<MatTable>().dataTable;

        nameText.text = st[weapon.name];
        atkText.text = weapon.atk.ToString();
        attackTypeText.text = weapon.property.ToString();
        priceText.text = $"비용 : {ct[item.id].gold}\n소지금 : {PlayDataManager.data.Gold}";

        if (ct[item.id].mf_module != -1)
        {
            var go = Instantiate(require, content.transform);
            var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id].mf_module);
            var count = 0;
            if (mat != null)
            {
                count = mat.count;
            }
            go.matText.text = st[mt[ct[item.id].mf_module].name];
            go.SetSlider(count, ct[item.id].mf_module_req);
            go.Renewal();
        }

        craftButton.gameObject.SetActive(IsCraftable());
    }

    public void CraftEquip()
    {
        if (!IsCraftable())
        {
            MyNotice.Instance.Notice("제작할 수 없습니다.");
            return;
        }

        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        var weapon = new Weapon(item.id);

        PlayDataManager.Purchase(ct[item.id].gold);
        PlayDataManager.DecreaseMat(ct[item.id].mf_module, ct[item.id].mf_module_req);
        PlayDataManager.AddItem(weapon);
        PlayDataManager.Save();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Renewal();
        }

        craftParticle.Play();

        CraftManager.Instance.ShowWeapons(true);
        gameObject.SetActive(false);
    }

    private bool IsCraftable()
    {
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;

        var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id].mf_module);
        if (mat == null)
        {
            //Debug.Log("Not Exist Materials");
            return false;
        }

        if (mat.count < ct[item.id].mf_module_req)
        {
            //Debug.Log("Lack Of Materials Count");
            return false;
        }

        if (PlayDataManager.data.Gold < ct[item.id].gold)
        {
            //Debug.Log("Lack Of Gold");
            return false;
        }

        if (PlayDataManager.data.WeaponInventory.Count > PlayDataManager.weaponsCapacity)
        {
            //Debug.Log("Full Of Inventory Count");
            return false;
        }

        return true;
    }
}
