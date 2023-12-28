using UnityEngine;

public class Skill : MonoBehaviour
{
    private PlayerStat stat;

    public void Init()
    {
        stat = GetComponent<Player>().Stat;
    }

    public int level { get; set; } = 1;
}

/*
public class TestSkill : Skill
{
    private void Start()
    {
        Init();
    }
}
*/