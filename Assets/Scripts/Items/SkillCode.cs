using UnityEngine;

public class SkillCode
{
    public int id = -1;
    public int count = 0;
    public bool isEquip = false;
    public readonly int Capacity = 99;

    public SkillCode(int id, int count = 0, bool isEquip = false)
    {
        this.id = id;
        this.count = count;
        this.isEquip = isEquip;
    }

    public void IncreaseCount(int value)
    {
        count = Mathf.Clamp(count + value, 1, Capacity);
    }
}