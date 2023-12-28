using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    private Dictionary<int, StageTable.Data> stageTable;
    private Dictionary<int, string> stringTable;
    //private Dictionary<int, EnemyTable.Data> enemyTable;

    [Header("Stage Container")]
    [SerializeField]
    private Transform stageContainer;

    [Header("Stage Prefab")]
    [SerializeField]
    private GameObject stagePrefab;

    [Header("Stage Info")]
    [SerializeField]
    private GameObject stageInfo;

    
    public Button categoryButton;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        if (stagePrefab == null)
        {
            return;
        }
        stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
        stringTable = CsvTableMgr.GetTable<StringTable>().dataTable;
        //enemyTable = CsvTableMgr.GetTable<EnemyTable>().dataTable;

        foreach (var info in stageTable)
        {
            var stageGameObject = Instantiate(stagePrefab);
            var stage = stageGameObject.GetComponent<Stage>();
            var data = info.Value;

            stage.title.text = stringTable[data.name];
            stage.description.text = stringTable[data.script];
            stage.enemyName.text = data.monster_id.ToString();
            //stage.enemyName.text = stringTable[enemyTable[data.monster_id].name];
            stage.button.onClick.AddListener(() =>
            {
                //stageInfo....text = asdf;
                stageInfo.SetActive(true);
            });
            stage.transform.SetParent(stageContainer, false);
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
}
