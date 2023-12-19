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

    [System.Serializable]
    private class PlayerData
    {
        public Player prefab;
        public GameObject infoUI;
        public Transform startTransform;
    }
    [System.Serializable]
    private class EnemyData
    {
        public EnemyAI prefab;
        public GameObject infoUI;
        public Transform startTransform;
    }

    [SerializeField] private PlayerData playerData;
    [SerializeField] private EnemyData enemyData;
    private Player player;
    private Slider playerHp;
    private EnemyAI enemy;
    private Slider enemyHp;
    private Slider evadePoint;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        playerData.infoUI.SetActive(false);
        enemyData.infoUI.SetActive(false);
        var prefabCheck = playerData.prefab == null || enemyData.prefab == null;
        var transformCheck = playerData.startTransform == null || enemyData.startTransform == null;
        if (prefabCheck)
        {
            //Debug.LogError($"Not Prefab!!\nPlayer: {playerData.prefab != null}, Enemy: {enemyData.prefab != null}");
            Destroy(this);
            return;
        }
        if (transformCheck)
        {
            //Debug.LogError($"Not Transfrom!!\nPlayer: {playerData.startTransform != null}, Enemy: {enemyData.startTransform != null}");
            Destroy(this);
            return;
        }
        player = Instantiate(playerData.prefab, playerData.startTransform.position, Quaternion.identity);
        enemy = Instantiate(enemyData.prefab, enemyData.startTransform.position, Quaternion.identity);
    }

    private void Start()
    {
        if (player == null || enemy == null)
        {
            return;
        }
        Init();
    }

    private void Update()
    {
        if (player == null || enemy == null)
        {
            return;
        }
        UpdateUI();
    }

    private void Init()
    {
        {
            playerData.infoUI.GetComponentInChildren<TextMeshProUGUI>().text = player.name;
            playerHp = playerData.infoUI.transform.Find("Hp").GetComponent<Slider>();
            playerHp.minValue = 0f;
            playerHp.maxValue = player.Stat.HP;
        }
        {
            enemyData.infoUI.GetComponentInChildren<TextMeshProUGUI>().text = enemy.name;
            //var st = CsvTableMgr.GetTable<StringTable>().dataTable;
            //enemyData.infoUI.GetComponentInChildren<TextMeshProUGUI>().text = st[enemy.ID];
            enemyHp = enemyData.infoUI.transform.Find("Hp").GetComponent<Slider>();
            enemyHp.minValue = 0f;
            enemyHp.maxValue = enemy.Stat.HP;
        }
        {
            evadePoint = enemyData.infoUI.transform.Find("EvadePoint").GetComponent<Slider>();
            evadePoint.minValue = 0f;
            evadePoint.maxValue = player.Stat.maxEvadePoint;
        }
        playerData.infoUI.SetActive(true);
        enemyData.infoUI.SetActive(true);
        
        UpdateUI();
    }

    public void UpdateUI()
    {
        playerHp.value = player.HP;
        enemyHp.value = enemy.HP;
        evadePoint.value = player.evadePoint;
    }
}
