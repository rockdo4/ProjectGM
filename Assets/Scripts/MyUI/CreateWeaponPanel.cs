using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateWeaponPanel : MonoBehaviour, IRenewal
{
    [Header("���� �̸� �ؽ�Ʈ")]
    [SerializeField]
    private TextMeshProUGUI nameText;

    [Header("������ �̹���")]
    public Image iconImage;

    [Header("���ݷ� �ؽ�Ʈ")]
    [SerializeField]
    private TextMeshProUGUI atkText;

    [Header("���� �Ӽ� �ؽ�Ʈ")]
    [SerializeField] 
    private TextMeshProUGUI attackTypeText;

    [Header("�䱸 ��� �г�")]
    [SerializeField]
    private RequireMatPanel require;

    [Header("�䱸 ��� �г� ����")]
    [SerializeField]
    private GameObject content;

    [Header("�䱸 �ݾ� �ؽ�Ʈ")]
    [SerializeField] 
    private TextMeshProUGUI priceText;

    [Header("�����ϱ� ��ư")]
    [SerializeField]
    private Button craftButton;

    [Header("���� ��ƼŬ")]
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
        priceText.text = $"��� : {ct[item.id].gold}\n������ : {PlayDataManager.data.Gold}";

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
            MyNotice.Instance.Notice("������ �� �����ϴ�.");
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
