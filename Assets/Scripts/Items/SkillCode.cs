using UnityEngine;

public class SkillCode
{
    public int id = -1;
    public int count = 0;
    public readonly int Capacity = 99;

    public SkillCode(int id, int count = 0)
    {
        this.id = id;
        this.count = count;
    }

    public void IncreaseCount(int value)
    {
        count = Mathf.Clamp(count + value, 1, Capacity);
    }
}