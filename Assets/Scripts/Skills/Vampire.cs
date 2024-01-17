using UnityEngine;

public class Vampire : Skill
{
    [Header("ü�� ���� ����")]
    [SerializeField]
    private float duration = 1.0f;

    private float timer = 0.0f;

    [Header("���� ����")]
    [SerializeField]
    private float drainValue = 0.03f;

    public Vampire(int id, int level)
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
        // Player Drain On
        player.Stat.drainRate = drainValue;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            timer = 0.0f;
            player.HP -= Mathf.RoundToInt(player.Stat.HP / 100);
        }
        
    }
}
