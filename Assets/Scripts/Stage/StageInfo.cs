using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageInfo : MonoBehaviour
{
    private Dictionary<int, StageTable.Data> stageTable;
    private Dictionary<int, string> stringTable;
    
    [Header("Enemy Icon 폴더경로")]
    private const string enemyIconPath = "sprites/Enemy Icon";

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI mapText;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Reward")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemPrefab;

    [Header("Icon")]
    [SerializeField] private IconSO matIconSo;
    [Header("Gold Icon Sprite")]
    [SerializeField] private Sprite goldIcon;
    //[Header("SkillCodeIconSo"), SerializeField]
    //private IconSO skillCodeIconSo;

    [Header("Image")]
    [SerializeField] private Image enemyImage;

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    private void OnEnable()
    {
        stageTable ??= CsvTableMgr.GetTable<StageTable>().dataTable;
        stringTable ??= CsvTableMgr.GetTable<StringTable>().dataTable;

        var stageID = PlayerPrefs.GetInt(PrefsKey.stageID);
        var stageInfo = stageTable[stageID];

        //Enemy
        enemyImage.sprite = Resources.Load<Sprite>($"{enemyIconPath}/{stringTable[stageInfo.iconName]}");
        
        //Texts
        titleText.text = stringTable[stageInfo.name];
        descriptionText.text = stringTable[stageInfo.script];
        mapText.text = ((StageManager.Maps)stageInfo.map_id).ToString();
        enemyText.text = stringTable[CsvTableMgr.GetTable<EnemyTable>().dataTable[stageInfo.monster_id].name];
        timerText.text = $"{TimeSpan.FromSeconds(stageInfo.time_limit).TotalMinutes}M";

        //Reward
        SetItemDic(stageInfo, out Dictionary<int, int> itemDic);
        foreach (var mat in itemDic)
        {
            MakeItem(matIconSo.GetSprite(mat.Key), mat.Value);
        }
        MakeItem(goldIcon, stageInfo.gold);

        //What....
        //var codeTable = CsvTableMgr.GetTable<CodeTable>().dataTable;
        //var codeKeys = new List<int>(codeTable.Keys);
        //var codeID = codeKeys[Random.Range(0, codeKeys.Count)];
        //MakeItem(skillCodeIconSo.GetSprite(codeTable[codeID].type), 1);

        //Button
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(((StageManager.Maps)stageInfo.map_id).ToString());
        });
    }
    private void OnDisable()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void MakeItem(Sprite sprite, int count)
    {
        var itemButton = Instantiate(itemPrefab, itemContainer).GetComponent<ItemButton>();
        itemButton.OnCountAct(true, count);
        itemButton.iconImage.sprite = sprite;
    }

    public void SetItemDic(StageTable.Data stageInfo, out Dictionary<int, int> itemDic)
    {
        itemDic = new Dictionary<int, int>();
        if (stageInfo.clear1 > 0 && !itemDic.ContainsKey(stageInfo.clear1))
        {
            itemDic.Add(stageInfo.clear1, stageInfo.clear1_count);
        }
        if (stageInfo.clear2 > 0 && !itemDic.ContainsKey(stageInfo.clear2))
        {
            itemDic.Add(stageInfo.clear2, stageInfo.clear2_count);
        }
        if (stageInfo.clear3 > 0 && !itemDic.ContainsKey(stageInfo.clear3))
        {
            itemDic.Add(stageInfo.clear3, stageInfo.clear3_count);
        }
        if (stageInfo.clear4 > 0 && !itemDic.ContainsKey(stageInfo.clear4))
        {
            itemDic.Add(stageInfo.clear4, stageInfo.clear4_count);
        }
        if (stageInfo.clear5 > 0 && !itemDic.ContainsKey(stageInfo.clear5))
        {
            itemDic.Add(stageInfo.clear5, stageInfo.clear5_count);
        }
    }
}
