using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    #region Player & Enemy
    [Header("Player And Enemy")]
    public EnemySO enemySO;

    [System.Serializable]
    private class PlayerInfo
    {
        [HideInInspector] public Player player;
        public GameObject infoUI;
        public TextMeshProUGUI nameText;
        public Slider hp;
        public Slider evadePoint;
        //public Transform startTransform;
    }
    [System.Serializable]
    private class EnemyInfo
    {
        [HideInInspector] public EnemyAI enemy;
        public GameObject infoUI;
        public Transform startTransform;
        public TextMeshProUGUI nameText;
        public Slider hp;
    }
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private EnemyInfo enemyInfo;
    #endregion

    [Header("Battle UI")]
    public GameObject battleUI;
    [Header("Timer")]
    public Slider limitTimer;

    #region Reward
    [Header("Reward")]
    public GameObject rewardUI;
    public Transform itemContainer;
    public GameObject itemPrefab;
    public IconSO matIconSo;
    public IconSO skillCodeIconSo;
    public GameObject itemInfo;
    #endregion

    private StageTable.Data stageData;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        var stageID = PlayerPrefs.GetInt("StageID");
        var stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;

        if (!stageTable.ContainsKey(stageID))
        {
            stageID = 1101001; //default;
            Debug.LogWarning("Not Found StageInfo!");
        }
        stageData = stageTable[stageID];

        InitPlayer();
        InitEnemy();
        SetTimer(stageData.time_limit);
    }

    private void Update()
    {
        if (playerInfo.player == null || enemyInfo.enemy == null)
        {
            return;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        limitTimer.value += Time.deltaTime;
        playerInfo.hp.value = playerInfo.player.HP;
        playerInfo.evadePoint.value = playerInfo.player.evadePoint;
        enemyInfo.hp.value = enemyInfo.enemy.HP;
    }

    public void Reward()
    {
        var stageID = PlayerPrefs.GetInt("StageID");
        var stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;

        //Material
        Dictionary<int, int> itemDic = new Dictionary<int, int>();
        if (stageData.clear1 > 0 && !itemDic.ContainsKey(stageData.clear1))
        {
            itemDic.Add(stageData.clear1, stageData.clear1_count);
        }
        if (stageData.clear2 > 0 && !itemDic.ContainsKey(stageData.clear2))
        {
            itemDic.Add(stageData.clear2, stageData.clear2_count);
        }
        if (stageData.clear3 > 0 && !itemDic.ContainsKey(stageData.clear3))
        {
            itemDic.Add(stageData.clear3, stageData.clear3_count);
        }
        if (stageData.clear4 > 0 && !itemDic.ContainsKey(stageData.clear4))
        {
            itemDic.Add(stageData.clear4, stageData.clear4_count);
        }

        foreach (var mat in itemDic)
        {
            PlayDataManager.IncreaseMat(mat.Key, mat.Value);
            MakeItem(matIconSo.GetSprite(mat.Key), mat.Value);
        }

        //Skill Code
        var codeTable = CsvTableMgr.GetTable<CodeTable>().dataTable;
        var codeKeys = new List<int>(codeTable.Keys);
        var codeID = codeKeys[Random.Range(0, codeKeys.Count)];
        PlayDataManager.IncreaseCode(codeID, 1);
        MakeItem(skillCodeIconSo.GetSprite(codeID), 1);

        //Gold
        PlayDataManager.AddGold(stageData.gold);
        MakeItem(null, stageData.gold);

        rewardUI.SetActive(true);
        PlayDataManager.StageUnlock(stageID);
    }
    public void MakeItem(Sprite sprite, int count)
    {
        var itemButton = Instantiate(itemPrefab, itemContainer).GetComponent<ItemButton>();
        itemButton.OnCountAct(true, count);
        itemButton.iconImage.sprite = sprite;
    }

    #region Player & Enemy
    private void InitPlayer()
    {
        if (playerInfo.player == null)
        {
            playerInfo.player = GameObject.FindWithTag(Tags.player).GetComponent<Player>();
        }
        playerInfo.nameText.text = playerInfo.player.name;

        playerInfo.hp.minValue = 0f;
        playerInfo.hp.maxValue = playerInfo.player.Stat.HP;
        playerInfo.hp.value = playerInfo.player.HP;

        playerInfo.evadePoint.minValue = 0f;
        playerInfo.evadePoint.maxValue = playerInfo.player.Stat.maxEvadePoint;
        playerInfo.evadePoint.value = 0f;
    }
    private void InitEnemy()
    {
        enemyInfo.enemy = enemySO.MakeEnemy(stageData.monster_id, enemyInfo.startTransform)?.GetComponent<EnemyAI>();
        if (enemyInfo.enemy == null)
        {
            enemyInfo.enemy = GameObject.FindWithTag(Tags.enemy).GetComponent<EnemyAI>();
        }
        enemyInfo.enemy.gameObject.SetActive(true);
        var stringTable = CsvTableMgr.GetTable<StringTable>().dataTable;

        enemyInfo.nameText.text = stringTable[CsvTableMgr.GetTable<EnemyTable>().dataTable[stageData.monster_id].name];
        enemyInfo.hp.minValue = 0f;
        enemyInfo.hp.maxValue = enemyInfo.enemy.Stat.HP;
        enemyInfo.hp.value = enemyInfo.enemy.HP;
    }
    #endregion

    #region Timer
    public void SetTimer(float time)
    {
        limitTimer.minValue = 0f;
        limitTimer.maxValue = time;
        limitTimer.value = 0f;
        limitTimer.onValueChanged.AddListener(OnTimerChanged);
    }
    private void OnTimerChanged(float value)
    {
        if (limitTimer.value >= limitTimer.maxValue)
        {
            GameManager.instance.GameOver(playerInfo.player);
        }
    }
    #endregion
}
