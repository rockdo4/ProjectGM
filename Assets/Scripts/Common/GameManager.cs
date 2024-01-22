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
    public GameObject cancelButton;

    public static readonly float pauseTimeScale = 0f;
    private float prevTimeScale = 1f;
    public bool IsGameOver { get; private set; }
    public bool IsPaused
    {
        get
        {
            return gameOverUI.activeSelf;
        }
    }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
        IsGameOver = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void EndGame()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = pauseTimeScale;
    }

    public void Win(EnemyAI enemy)
    {    
        Pause(true);
        gameOverUI.SetActive(false);
        InGameManager.Instance.Reward();
    }

    public void Lose(Player player)
    {
        Pause(true);
    }

    public void GameOver(LivingObject deathObject)
    {
        if (cancelButton != null)
        {
            cancelButton.SetActive(false);
        }

        if (deathObject is Player)
        {
            Lose(deathObject as Player);
        }
        else
        {
            Win(deathObject as EnemyAI);
        }
        IsGameOver = true;
    }

    public void Pause(bool active)
    {
        if (IsGameOver)
        {
            return;
        }
        TouchManager.Instance.enabled = !active;
        prevTimeScale = (Time.timeScale == pauseTimeScale) ? 1f : Time.timeScale;
        gameOverUI.SetActive(active);
        Time.timeScale = active ? pauseTimeScale : prevTimeScale;
    }
    public void Pause()
    {
        if (IsGameOver)
        {
            return;
        }
        bool active = gameOverUI.activeSelf;
        prevTimeScale = (Time.timeScale == pauseTimeScale) ? 1f : Time.timeScale;
        TouchManager.Instance.enabled = active;
        gameOverUI.SetActive(!active);
        Time.timeScale = !active ? pauseTimeScale : prevTimeScale;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}