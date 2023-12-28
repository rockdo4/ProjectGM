using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour, IRenewal
{
    [Header("체력 텍스트")]
    [SerializeField]
    private TextMeshProUGUI healthText;

    [Header("세트효과 이미지")]
    [SerializeField]
    private Image[] setSkillImages;

    [Header("세트스킬 텍스트")]
    [SerializeField]
    private TextMeshProUGUI setSkillText;

    [Header("무기")]
    [SerializeField]
    private TextMeshProUGUI currentWeaponText;

    [SerializeField]
    private TextMeshProUGUI attackTypeText;

    [SerializeField]
    private TextMeshProUGUI attackValueText;

    [Header("방어구")]
    [SerializeField]
    private TextMeshProUGUI[] armorsText;

    [Header("방어력 텍스트")]
    [SerializeField]
    private TextMeshProUGUI defenceValueText;

    public void Renewal()
    {
        gameObject.SetActive(true);

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        HealthRenewal();
        SetSkillRenewal();
        WeaponRenewal();
        ArmorRenewal();
    }

    private void HealthRenewal()
    {
        // healthText.text = 
    }

    private void SetSkillRenewal()
    {
        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;
        var at = CsvTableMgr.GetTable<ArmorTable>().dataTable;
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        // Color Reset
        foreach (var image in setSkillImages)
        {
            image.color = Color.white;
        }

        setSkillText.text = (PlayDataManager.curSetSkill.id == -1) ? 
            string.Empty : 
            st[skt[PlayDataManager.curSetSkill.id].name];

        if (PlayDataManager.curSetSkill.id == -1)
        {
            return;
        }

        var count = Mathf.Clamp(PlayDataManager.curSetSkill.level, 0, 3);
        for (int i = 0; i < count; i++)
        {
            setSkillImages[i].color = Color.cyan;
        }
    }

    private void WeaponRenewal()
    {
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        var wt = CsvTableMgr.GetTable<WeaponTable>().dataTable;

        currentWeaponText.text = st[wt[PlayDataManager.curWeapon.id].name];
        attackTypeText.text = PlayDataManager.curWeapon.attackType.ToString();
        attackValueText.text = wt[PlayDataManager.curWeapon.id].atk.ToString();
    }

    private void ArmorRenewal()
    {
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        var at = CsvTableMgr.GetTable<ArmorTable>().dataTable;

        int index = 0;
        int value = 0;
        foreach (var armor in PlayDataManager.curArmor.Values)
        {
            if (armor == null)
            {
                armorsText[index].text = string.Empty;

                index++;
                continue;
            }
            armorsText[index].text = st[at[armor.id].name];
            value += at[armor.id].defence;
            index++;
        }

        defenceValueText.text = value.ToString();
    }

    private void OnEnable()
    {
        Renewal();
    }
}
