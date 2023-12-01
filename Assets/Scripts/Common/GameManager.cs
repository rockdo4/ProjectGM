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

    private void Awake()
    {
        Time.timeScale = 1f;
        if (instance != this)
        {
            Destroy(gameObject);
        }
        gameOverUI.SetActive(false);
    }


    public void EndGame()
    {
        //Time.timeScale = gameOverTimeScale;
        gameOverUI.SetActive(true);
    }
}