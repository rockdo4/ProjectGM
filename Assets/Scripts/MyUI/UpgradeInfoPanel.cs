using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UpgradeInfoPanel : MonoBehaviour, IRenewal
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Renewal()
    {
        PrintAtk();
        PrintDef();
    }

    private void PrintAtk()
    {
        if (PlayDataManager.curWeapon == null)
        {
            text.text = $"[공격력] : 0\n[무기 속성] : {AttackType.None}";
            return;
        }

        var wt = CsvTableMgr.GetTable<WeaponTable>().dataTable;
        var weapon = wt[PlayDataManager.curWeapon.id];

        // 공격력 합산식
        var atk = 0.0f;
        if (weapon != null)
        {
            atk = weapon.atk;
        }

        // 무기 속성
        var attackType = AttackType.None;
        if (weapon != null)
        {
            attackType = weapon.property;
        }

        text.text = $"[공격력] : {atk}\n[무기 속성] : {attackType}";
    }

    private void PrintDef()
    {
        var at = CsvTableMgr.GetTable<ArmorTable>().dataTable;

        // 방어력 합산식
        var def = 0;
        foreach (var armor in PlayDataManager.curArmor)
        {
            if (armor.Value != null)
            {
                def += at[armor.Value.id].defence;
            }
        }

        text.text += $"\n[방어력] {def}";
    }
}
