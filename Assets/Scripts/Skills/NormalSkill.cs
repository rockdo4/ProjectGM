using UnityEngine;

public class NormalSkill : Skill
{
    public NormalSkill(int id, int level)
        : base(id, level)
    {

    }

    private void Start()
    {
        Init();

        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        var skill = CsvTableMgr.GetTable<SkillTable>().dataTable[id];
        switch (st[skill.name])
        {
            case "���ݷ�":
                player.Stat.AttackDamage += Mathf.RoundToInt(skill.value * level);
                break;

            case "����":
                player.Stat.Defence += Mathf.RoundToInt(skill.value * level);
                break;

            case "ü��":
                player.Stat.HP += Mathf.RoundToInt(skill.value * level);
                player.HP = player.Stat.HP;
                InGameManager.Instance.SetHP(player.HP, player.Stat.HP);
                break;

            case "Ÿ����":
                player.CurrentWeapon.weakDamage += skill.value * level;
                break;

            case "���":
                player.Stat.evadePoint += skill.value * level;
                break;

            default:
                break;
        }
    }
}