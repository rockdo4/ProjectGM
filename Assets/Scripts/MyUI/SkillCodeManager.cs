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

    [Header("착용중 Content")]
    [SerializeField]
    private GameObject equipContent;

    [Header("인벤토리 Content")]
    [SerializeField]
    private GameObject invContent;

    [Header("버튼 프리팹")]
    [SerializeField]
    private ItemButton buttonPrefab;

    [Header("잠금버튼 프리팹")]
    [SerializeField]
    private GameObject lockPrefab;

    [Header("스킬코드 장착 패널")]
    [SerializeField]
    private SkillCodeEquipPanel equipPanel;

    private ObjectPool<ItemButton> buttonPool;
    private List<ItemButton> releaseList = new List<ItemButton>();

    private ObjectPool<GameObject> lockPool;
    private List<GameObject> lockList = new List<GameObject>();

    private void Start()
    {
        buttonPool = new ObjectPool<ItemButton>
            (() => // createFunc
            {
                var button = Instantiate(buttonPrefab, invContent.transform);
                button.Clear();
                button.gameObject.SetActive(false);

                return button;
            },
            delegate (ItemButton button) // actionOnGet
            {
                button.gameObject.SetActive(true);
                button.transform.SetParent(invContent.transform, true);
            },
            delegate (ItemButton button) // actionOnRelease
            {
                button.Clear();
                button.transform.SetParent(gameObject.transform, true); // ItemButton Transform Reset
                button.gameObject.SetActive(false);
            });

        lockPool = new ObjectPool<GameObject>
            (() => // createFunc
            {
                var go = Instantiate(lockPrefab, equipContent.transform);
                go.SetActive(false);
                return go;
            },
            delegate (GameObject go) // actionOnGet
            {
                go.SetActive(true);
                go.transform.SetParent(equipContent.transform, true);
            },
            delegate (GameObject go) // actionOnRelease
            {
                go.transform.SetParent(gameObject.transform, true);
                go.SetActive(false);
            });

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        Renewal();
    }

    public void ShowAll()
    {
        ClearItemButton();

        var table = CsvTableMgr.GetTable<CodeTable>().dataTable;
        foreach (var code in PlayDataManager.data.CodeInventory)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = skillcodeIconSO.GetSprite(table[code.id].type);
            go.OnCountAct(true, code.count);

            go.button.onClick.AddListener(() =>
            {
                equipPanel.iconImage.sprite = go.iconImage.sprite;
                equipPanel.SetSkillCode(code);
                equipPanel.EquipMode();
                equipPanel.Renewal();
            });
            releaseList.Add(go);
        }
    }

    public void ShowEquip()
    {
        ClearLock();

        var table = CsvTableMgr.GetTable<CodeTable>().dataTable;
        foreach (var id in PlayDataManager.data.SkillCodes)
        {
            var code = new SkillCode(id, 1);
            var go = buttonPool.Get();

            go.iconImage.sprite = skillcodeIconSO.GetSprite(table[id].type);
            go.transform.SetParent(equipContent.transform, true);
            go.OnCountAct();

            go.button.onClick.AddListener(() =>
            {
                equipPanel.iconImage.sprite = go.iconImage.sprite;
                equipPanel.SetSkillCode(code);
                equipPanel.EquipMode(false);
                equipPanel.Renewal();
            });
            releaseList.Add(go);
        }

        for (int i = PlayDataManager.GetSocket() - PlayDataManager.data.SkillCodes.Count; i > 0; i--)
        {
            var go = lockPool.Get();

            lockList.Add(go);
        }
    }

    private void Clear()
    {
        ClearItemButton();
        ClearLock();
    }

    private void ClearItemButton()
    {
        foreach (var item in releaseList)
        {
            buttonPool.Release(item);
        }

        releaseList.Clear();
    }

    private void ClearLock()
    {
        foreach (var item in lockList)
        {
            lockPool.Release(item);
        }

        lockList.Clear();
    }

    public void Renewal()
    {
        gameObject.SetActive(true);
        Clear();

        ShowAll(); // test code
        ShowEquip();
    }
}
