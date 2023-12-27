using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SkillSO")]
public class SkillSO : ScriptableObject
{
    [Header("스킬 ID")]
    [SerializeField]
    private List<int> ID;

    [Header("스킬")]
    [SerializeField]
    private List<Skill> SKILL;

    public Skill GetSkill(int id, int level)
    {
        var index = ID.FindIndex(x => x == id);

        return (index == -1) ? null : SKILL[index];
    }
}