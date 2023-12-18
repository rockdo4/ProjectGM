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
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
        IsGameOver = false;
        Pause(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!gameOverUI.activeSelf);
        }
    }

    public void EndGame()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = gameOverTimeScale;
    }

    public void Win(EnemyAI enemy)
    {
        Pause(true);
    }

    public void Lose(Player player)
    {
        Pause(true);
    }

    public void GameOver(LivingObject deathObject)
    {
        IsGameOver = true;
        if (deathObject is Player)
        {
            Lose(deathObject as Player);
        }
        else
        {
            Win(deathObject as EnemyAI);
        }
    }

    public void Pause(bool active)
    {
        if (!active && IsGameOver)
        {
            return;
        }
        gameOverUI.SetActive(active);
        Time.timeScale = active ? gameOverTimeScale : originalTimeScale;
    }
}