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
            case "공격력":
                player.Stat.AttackDamage += Mathf.RoundToInt(skill.value * level);
                break;

            case "방어력":
                player.Stat.Defence += Mathf.RoundToInt(skill.value * level);
                break;

            case "체력":
                player.Stat.HP += Mathf.RoundToInt(skill.value * level);
                player.HP = player.Stat.HP;
                break;

            case "타격점":
                player.CurrentWeapon.weakDamage += skill.value * level;
                break;

            case "농락":
                player.Stat.evadePoint += skill.value * level;
                break;

            default:
                break;
        }
    }
}