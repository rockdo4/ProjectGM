using UnityEngine;

public class WildTurf : Skill
{
    public WildTurf(int id, int level) 
        : base(id, level)
    {

    }

    private void Start()
    {
        Init();

        player.Stat.Defence += Mathf.RoundToInt(player.Stat.Defence * 0.3f);
    }
}