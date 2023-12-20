using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkillCodeManager : MonoBehaviour, IRenewal
{
    //[Header("스킬코드 패널")]
    //public SkillCodePanel skillCodePanel
    [Header("스킬코드 IconSO")]
    [SerializeField]
    private IconSO skillcodeIconSO;

    [Header("Content")]
    [SerializeField]
    private GameObject content;

    [Space(10.0f)]

    [Header("버튼 프리팹")]
    [SerializeField]
    private ItemButton buttonPrefab;
    private ObjectPool<ItemButton> buttonPool;
    private List<ItemButton> releaseList = new List<ItemButton>();

    private void Start()
    {
        buttonPool = new ObjectPool<ItemButton>(
            () => // createFunc
            {
                var button = Instantiate(buttonPrefab);
                button.transform.SetParent(content.transform, true);
                button.Clear();
                button.gameObject.SetActive(false);

                return button;
            },
        delegate (ItemButton button) // actionOnGet
        {
            button.gameObject.SetActive(true);
            button.transform.SetParent(content.transform, true);
        },
        delegate (ItemButton button) // actionOnRelease
        {
            button.Clear();
            button.transform.SetParent(gameObject.transform, true); // ItemButton Transform Reset
            button.gameObject.SetActive(false);
        });

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        ShowAll();
    }

    public void ShowAll()
    {
        ClearItemButton();

        var codes = PlayDataManager.data.CodeInventory;
        var table = CsvTableMgr.GetTable<CodeTable>().dataTable;
        foreach (var code in codes)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = skillcodeIconSO.GetSprite(table[code.id].type);

            go.button.onClick.AddListener(() =>
            {

            });
            releaseList.Add(go);
        }
    }

    public void ClearItemButton()
    {
        foreach (var item in releaseList)
        {
            buttonPool.Release(item);
        }

    }

    public void Renewal()
    {
        gameObject.SetActive(true);
    }
}
