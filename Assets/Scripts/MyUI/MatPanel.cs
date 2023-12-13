using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatPanel : MonoBehaviour, IRenewal
{
    [Header("재료 이름")]
    [SerializeField]
    private TextMeshProUGUI nameText;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("판매 가격")]
    [SerializeField]
    private TextMeshProUGUI sellText;

    [Header("설명")]
    [SerializeField]
    private TextMeshProUGUI infoText;

    [Header("개수")]
    [SerializeField]
    private TextMeshProUGUI countText;

    private Materials mat;

    public void SetMaterials(Materials mat)
    {
        this.mat = mat;
    }

    public void Renewal()
    {
        gameObject.SetActive(true);

        var table = CsvTableMgr.GetTable<MatTable>().dataTable;
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        nameText.text = st[table[mat.id].item_name];
        infoText.text = st[table[mat.id].item_script];
        sellText.text = $"판매 가격 : {table[mat.id].gold.ToString()}";
        countText.text = $"{mat.count} / {mat.Capacity}";
    }

    public void SellItem()
    {
        PlayDataManager.SellItem(mat, 1);

        Renewal();
    }
}