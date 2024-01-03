using UnityEngine;

public class BrokenShield : Skill
{
    public BrokenShield(int id, int level) 
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

    }
}
