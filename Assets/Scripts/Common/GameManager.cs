using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    private static GameManager m_instance;

    public GameObject gameOverUI;
    public static readonly float gameOverTimeScale = 0f;
    public static readonly float originalTimeScale = 1f;
    private void Awake()
    {
        Time.timeScale = 1f;
        if (instance != this)
        {
            Destroy(gameObject);
        }
        gameOverUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOverUI.activeSelf)
            {
                Time.timeScale = originalTimeScale;
                gameOverUI.SetActive(false);
            }
            else
            {
                EndGame();
            }
        }
    }

    public void EndGame()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = gameOverTimeScale;
    }

    public void Win(EnemyAI enemy)
    {
        Debug.Log($"{enemy.name} 몬스터 때려잡았음");
        EndGame();
    }

    public void Lose(Player player)
    {
        Debug.Log($"{player.Stat.AttackDamage + player.CurrentWeapon.attack}공격력으로 졌다...");
        EndGame();
    }
}