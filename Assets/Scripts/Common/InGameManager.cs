using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<InGameManager>();
            }
            return m_instance;
        }
    }
    private static InGameManager m_instance;

    [Header("EnemySO")]
    public EnemySO enemySO;

    [System.Serializable]
    private class PlayerData
    {
        public Player player;
        public GameObject infoUI;
        public Transform startTransform;
    }
    [System.Serializable]
    private class EnemyData
    {
        public EnemyAI enemy { get; set; }
        public GameObject infoUI;
        public Transform startTransform;
    }

    [SerializeField] private PlayerData playerData;
    [SerializeField] private EnemyData enemyData;
    private Slider playerHp;
    private Slider enemyHp;
    private Slider evadePoint;
    private EnemyTable.Data enemyInfo;

    [SerializeField] public Slider timeSlider;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        var stageID = PlayerPrefs.GetInt("StageID");
        var stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
#if UNITY_EDITOR
        if (!stageTable.ContainsKey(stageID))
        {
            stageID = 1101001; //default;
        }
#endif
        if (!stageTable.ContainsKey(stageID))
        {
            Debug.LogWarning("Not Found StageInfo!");
        }

        var stageInfo = stageTable[stageID];

        timeSlider.minValue = 0f;
        timeSlider.maxValue = stageInfo.time_limit;
        timeSlider.value = 0f;
        timeSlider.onValueChanged.AddListener(OnTimerChanged);

        playerData.infoUI.SetActive(false);
        enemyData.infoUI.SetActive(false);

        enemyInfo = CsvTableMgr.GetTable<EnemyTable>().dataTable[stageInfo.monster_id];
        enemyData.enemy = enemySO.MakeEnemy(stageInfo.monster_id, enemyData.startTransform)?.GetComponent<EnemyAI>();
        if (enemyData.enemy == null)
        {
            enemyData.enemy = GameObject.FindWithTag(Tags.enemy).GetComponent<EnemyAI>();
        }
        if (playerData.player == null)
        {
            playerData.player = GameObject.FindWithTag(Tags.player).GetComponent<Player>();
        }
        enemyData.enemy.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (playerData.player == null || enemyData.enemy == null)
        {
            return;
        }
        Init();
    }

    private void Update()
    {
        if (playerData.player == null || enemyData.enemy == null)
        {
            return;
        }
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.V))
        {
            timeSlider.value += 60f;
        }
    }

    private void Init()
    {
        {
            playerData.infoUI.GetComponentInChildren<TextMeshProUGUI>().text = playerData.player.name;
            playerHp = playerData.infoUI.transform.Find("Hp").GetComponent<Slider>();
            playerHp.minValue = 0f;
            playerHp.maxValue = playerData.player.Stat.HP;
        }
        {
            var st = CsvTableMgr.GetTable<StringTable>().dataTable;
            enemyData.infoUI.GetComponentInChildren<TextMeshProUGUI>().text = st[enemyInfo.name];
            enemyHp = enemyData.infoUI.transform.Find("Hp").GetComponent<Slider>();
            enemyHp.minValue = 0f;
            enemyHp.maxValue = enemyData.enemy.Stat.HP;
        }
        {
            evadePoint = enemyData.infoUI.transform.Find("EvadePoint").GetComponent<Slider>();
            evadePoint.minValue = 0f;
            evadePoint.maxValue = playerData.player.Stat.maxEvadePoint;
        }
        playerData.infoUI.SetActive(true);
        enemyData.infoUI.SetActive(true);
        
        UpdateUI();
    }

    public void UpdateUI()
    {
        timeSlider.value += Time.deltaTime;
        playerHp.value = playerData.player.HP;
        enemyHp.value = enemyData.enemy.HP;
        evadePoint.value = playerData.player.evadePoint;
    }

    private void OnTimerChanged(float value)
    {
        if (timeSlider.value >= timeSlider.maxValue)
        {
            GameManager.instance.GameOver(playerData.player);
        }
    }
}
