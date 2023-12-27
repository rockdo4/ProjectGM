using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("BLACK")]
    [SerializeField]
    private FadeEffects BLACK;

    private Dictionary<int, StageTable.Data> stageTable = new Dictionary<int, StageTable.Data>();
    private Dictionary<int, string> stringTable;

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        stageTable = CsvTableMgr.GetTable<StageTable>().dataTable;
        stringTable = CsvTableMgr.GetTable<StringTable>().dataTable;

        //foreach(var stage in stageTable)
        //{
        //    Debug.Log(stage.Key);
        //    Debug.Log("---------------------------");
        //    foreach (var data in stringTable[stage.Value.name])
        //    {
        //        Debug.Log(data);
        //    }
        //    Debug.Log("---------------------------");
        //}
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
