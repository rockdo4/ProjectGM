using UnityEngine;

public class ExpertHunter : Skill
{
    public ExpertHunter(int id, int level)
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

        var enemy = GameObject.FindWithTag(Tags.enemy);
        var weak = enemy.GetComponent<EnemyAI>().Stat.weaknessType;
        player.CurrentWeapon.type = weak;
    }
}
