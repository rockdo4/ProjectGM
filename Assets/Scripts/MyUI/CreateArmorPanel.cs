using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateArmorPanel : MonoBehaviour, IRenewal
{
    [Header("�� �̸� �ؽ�Ʈ")]
    [SerializeField] 
    private TextMeshProUGUI nameText;

    [Header("������ �̹���")]
    public Image iconImage;

    [Header("���� �ؽ�Ʈ")]
    [SerializeField]
    private TextMeshProUGUI defText;

    [Header("��ų1 �ؽ�Ʈ")]
    [SerializeField]
    private TextMeshProUGUI skill1Text;

    [Header("��ų2 �ؽ�Ʈ")]
    [SerializeField]
    private TextMeshProUGUI skill2Text;

    [Header("��Ʈ ��ų �ؽ�Ʈ")]
    [SerializeField]
    private TextMeshProUGUI setSkillText;

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

        var armor = CsvTableMgr.GetTable<ArmorTable>().dataTable[item.id];
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        var mt = CsvTableMgr.GetTable<MatTable>().dataTable;
        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;

        nameText.text = st[armor.name];
        defText.text = armor.defence.ToString();
        setSkillText.text = (armor.set_skill_id == -1) ? 
            string.Empty :
            st[skt[armor.set_skill_id].name].ToString();

        skill1Text.text = (armor.skill1_id == -1) ? 
            string.Empty : 
            $"{st[skt[armor.skill1_id].name]} Lv.{armor.skill1_lv}\n";

        skill2Text.text = (armor.skill2_id == -1) ? 
            string.Empty : 
            $"{st[skt[armor.skill2_id].name]} Lv.{armor.skill2_lv}";

        priceText.text = $"��� : {ct[item.id].gold}\n������ : {PlayDataManager.data.Gold}";

        if (ct[item.id].mf_module != -1) // �䱸 ��Ḷ�� �б�
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

        if (ct[item.id].mon_core != -1)
        {
            var go = Instantiate(require, content.transform);
            var mat = PlayDataManager.data.MatInventory.Find(x => x.id == ct[item.id].mon_core);
            var count = 0;
            if (mat != null)
            {
                count = mat.count;
            }
            go.matText.text = st[mt[ct[item.id].mon_core].name];
            go.SetSlider(count, ct[item.id].mon_core_req);
            go.Renewal();
        }

        craftButton.gameObject.SetActive(IsCraftable());
    }

    public void CraftEquip()
    {
        if (!IsCraftable())
        {
            // ���� �Ұ���
            return;
        }

        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        var armor = new Armor(item.id);

        PlayDataManager.Purchase(ct[item.id].gold);
        PlayDataManager.DecreaseMat(ct[item.id].mf_module, ct[item.id].mf_module_req);
        PlayDataManager.data.ArmorInventory.Add(armor);
        PlayDataManager.Save();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Renewal();
        }

        craftParticle.Play();

        CraftManager.Instance.ShowArmors(true);
        gameObject.SetActive(false);
    }

    private bool IsCraftable()
    {
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

        if (PlayDataManager.data.ArmorInventory.Count > PlayDataManager.armorsCapacity)
        {
            //Debug.Log("Full Of Inventory Count");
            return false;
        }

        return true;
    }
}
