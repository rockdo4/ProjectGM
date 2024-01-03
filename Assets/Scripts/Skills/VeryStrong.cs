using UnityEngine;

public class VeryStrong : Skill
{
    public VeryStrong(int id, int level) 
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

        player.Stat.attackFinalRate = 0.3f;
    }
}
