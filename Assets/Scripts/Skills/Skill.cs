using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player player;

    protected int id { get; private set; } = -1;

    protected int level { get; set; } = 1;

    protected void Init()
    {
        player = GetComponentInParent<Player>();
    }

    public Skill(int id, int level)
    {
        this.id = id;
        this.level = level;
    }

    public virtual void SetSkill(int id, int level)
    {
        this.id = id;
        this.level = level;
    }
}