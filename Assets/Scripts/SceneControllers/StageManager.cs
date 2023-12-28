using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public enum Maps
    {
        Zoo = 1,
        RPG,
        Horror
    }

    public enum Type
    {
        Global = 1,
        Local
    }

    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    private Dictionary<int, StageTable.Data> stageTable;
    private Dictionary<int, string> stringTable;
    private Dictionary<int, EnemyTable.Data> enemyTable;

    [Header("Defualt Category")]
    [SerializeField]
    private int defaultCategory;

    [Header("Stage Container")]
    [SerializeField]
    private Transform stageContainer;

    [Header("Stage Prefab")]
    [SerializeField]
    private GameObject stagePrefab;

    [Header("Stage Info")]
    [SerializeField]
    private GameObject stageInfo;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        PlayerPrefs.DeleteKey("StageID");

        if (stagePrefab == null)
        {
            return;
        }
        stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
        stringTable = CsvTableMgr.GetTable<StringTable>().dataTable;
        enemyTable = CsvTableMgr.GetTable<EnemyTable>().dataTable;

        foreach (var info in stageTable)
        {
            var stageGameObject = Instantiate(stagePrefab);
            var stage = stageGameObject.GetComponent<Stage>();
            var data = info.Value;

            stage.id = info.Key;
            stage.type = data.type;
            stage.title.text = stringTable[data.name];
            var path = $"sprites/Enemy Icon/{stringTable[data.iconName]}";
            stage.image.sprite = Resources.Load<Sprite>(path);
            stage.mapName.text = ((Maps)data.map_id).ToString();
            stage.enemyName.text = stringTable[enemyTable[data.monster_id].name];
            stage.button.onClick.AddListener(() =>
            {
                //stageInfo....text = asdf;
                PlayerPrefs.SetInt("StageID", info.Key);
                SceneManager.LoadScene(((Maps)data.map_id).ToString());
                if (stageInfo != null)
                {
                    stageInfo.SetActive(true);
                }
            });
            stage.transform.SetParent(stageContainer, false);
        }

        CategoryFilter(defaultCategory);
    }

    public void GoGame(string sceneName)
    {
        if (PlayDataManager.curWeapon == null)
        {
            MyNotice.Instance.Notice("무기를 먼저 장착해주십시오.");
            return;
        }
        BLACK.FadeOut(sceneName);
    }

    public void CategoryFilter(int type)
    {
        var length = stageContainer.childCount;
        for (int i = 0; i < length; i++)
        {
            var stage = stageContainer.GetChild(i).GetComponent<Stage>();
            if (stage == null)
            {
                continue;
            }

            if (stage.type == type)
            {
                stage.gameObject.SetActive(true);
            }
            else
            {
                stage.gameObject.SetActive(false);
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CategoryFilter(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CategoryFilter(2);
        }
    }
}
