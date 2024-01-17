using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public enum Maps
    {
        Zoo = 1,
        RPG,
        Horror
    }

    [Header("Enemy Icon 폴더경로")]
    [SerializeField]
    private const string enemyIconPath = "sprites/Enemy Icon";

    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    [Header("Global Panel")]
    [SerializeField]
    private GameObject globalPanel;

    [Header("Local Panel")]
    [SerializeField]
    private GameObject localPanel;

    [Header("Stage Container")]
    [SerializeField]
    private Transform stageContainer;

    [Header("Stage Content Prefab")]
    [SerializeField]
    private GameObject stagePrefab;

    [Header("Area")]
    [SerializeField] private GameObject categoryArea;
    [SerializeField] private GameObject stageArea;
    [SerializeField] private GameObject stageInfoArea;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        PlayDataManager.StageInfoRefresh();

        if (stagePrefab == null)
        {
            return;
        }

        var stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
        var stringTable = CsvTableMgr.GetTable<StringTable>().dataTable;
        var enemyTable = CsvTableMgr.GetTable<EnemyTable>().dataTable;

        foreach (var info in stageTable)
        {
            var stageGameObject = Instantiate(stagePrefab);
            var stage = stageGameObject.GetComponent<Stage>();
            var data = info.Value;

            stage.id = info.Key;
            stage.type = data.type;
            stage.title.text = stringTable[data.name];
            stage.image.sprite = Resources.Load<Sprite>($"{enemyIconPath}/{stringTable[data.iconName]}");
            stage.mapName.text = ((Maps)data.map_id).ToString();
            stage.enemyName.text = stringTable[enemyTable[data.monster_id].name];
            stage.button.onClick.AddListener(() =>
            {
                TempVariable.stageID = stage.id;
                if (stageInfoArea != null)
                {
                    stageArea.SetActive(false);
                    stageInfoArea.SetActive(true);
                }
                else
                {
                    SceneManager.LoadScene(((Maps)data.map_id).ToString());
                }
            });
            stage.transform.SetParent(stageContainer, false);
            if (!PlayDataManager.StageUnlockCheck(info.Key))
            {
                stage.button.interactable = false;
            }
        }

        if (PlayDataManager.AllClearedCheck(1))
        {
            globalPanel.transform.Find("Clear Panel").gameObject.SetActive(true);
        }
        if (PlayDataManager.AllClearedCheck(2))
        {
            localPanel.transform.Find("Clear Panel").gameObject.SetActive(true);
        }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stageInfoArea.activeSelf)
            {
                stageInfoArea.SetActive(false);
                stageArea.SetActive(true);
            }
            else if (stageArea.activeSelf)
            {
                stageArea.SetActive(false);
                categoryArea.SetActive(true);
            }
            else
            {
                BLACK.FadeOut("Title");
            }
        }
    }
}
