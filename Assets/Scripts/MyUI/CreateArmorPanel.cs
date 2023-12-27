using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateArmorPanel : MonoBehaviour, IRenewal
{
    [Header("방어구 이름 텍스트")]
    public TextMeshProUGUI nameText;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("방어력 텍스트")]
    public TextMeshProUGUI defText;

    [Header("스킬1 텍스트")]
    public TextMeshProUGUI skill1Text;

    [Header("스킬2 텍스트")]
    public TextMeshProUGUI skill2Text;

    [Header("세트 스킬 텍스트")]
    public TextMeshProUGUI setSkillText;

    [Header("요구 재료 패널")]
    public RequireMatPanel require;

    [Header("요구 재료 패널 영역")]
    [SerializeField]
    private GameObject content;

    [Header("요구 금액 텍스트")]
    [SerializeField]
    private TextMeshProUGUI priceText;



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

        priceText.text = $"비용 : {ct[item.id].gold}\n소지금 : {PlayDataManager.data.Gold}";

        if (ct[item.id].mf_module != -1) // 요구 재료마다 분기
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

        if (ct[item.id].mon_core != -1) // 요구 재료마다 분기
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
    }

    public void CraftEquip()
    {
        if (!IsCraftable())
        {
            // 제작 불가능
            return;
        }

        var ct = CsvTableMgr.GetTable<CraftTable>().dataTable;
        var armor = new Armor(item.id);

        PlayDataManager.Purchase(ct[item.id].gold);
        PlayDataManager.DecreaseMat(ct[item.id].mf_module, ct[item.id].mf_module_req);
        PlayDataManager.data.ArmorInventory.Add(armor);
        PlayDataManager.Save();

        CraftManager.Instance.ShowArmors(true);
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
        // 인벤토리 공간 부족 (추후 추가 필요)


        return true;
    }
}
