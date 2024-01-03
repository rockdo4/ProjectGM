using UnityEngine;

public class HeavyBlow : Skill
{
    public HeavyBlow(int id, int level) 
        : base(id, level)
    {

    }

    private void Start()
    {
        Init();

        if (level < 3)
        {
            gameObject.SetActive(false);
            Debug.Log(gameObject.name + " OFF");
            return;
        }

        player.Stat.attackEvadePointRate = 0.02f;
    }
}
