using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkillCodeManager : MonoBehaviour, IRenewal
{
    [Header("스킬 정보 프리팹")]
    [SerializeField]
    private SkillCodeInfoPanel infoPrefab;

    [Header("스킬 정보 Content")]
    [SerializeField]
    private GameObject infoContent;

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
    private LockerButton lockPrefab;

    [Header("스킬코드 장착 패널")]
    [SerializeField]
    private SkillCodeEquipPanel equipPanel;

    private ObjectPool<ItemButton> buttonPool;
    private List<ItemButton> releaseList = new List<ItemButton>();

    private ObjectPool<LockerButton> lockPool;
    private List<LockerButton> lockList = new List<LockerButton>();

    private ObjectPool<SkillCodeInfoPanel> infoPool;
    private List<SkillCodeInfoPanel> infoList = new List<SkillCodeInfoPanel>();

    private void Awake()
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

        lockPool = new ObjectPool<LockerButton>
            (() => // createFunc
            {
                var go = Instantiate(lockPrefab, equipContent.transform, true);
                go.gameObject.SetActive(false);
                return go;
            },
            delegate (LockerButton go) // actionOnGet
            {
                go.gameObject.SetActive(true);
                go.transform.SetParent(equipContent.transform, true);
            },
            delegate (LockerButton go) // actionOnRelease
            {
                go.transform.SetParent(gameObject.transform, true);
                go.gameObject.SetActive(false);
            });

        infoPool = new ObjectPool<SkillCodeInfoPanel>
            (() => // createFunc
            {
                var go = Instantiate(infoPrefab, infoContent.transform, true);
                go.gameObject.SetActive(false);
                return go;
            },
            delegate (SkillCodeInfoPanel go)
            {
                go.gameObject.SetActive(true);
                go.transform.SetParent(infoContent.transform, true);
            },
            delegate (SkillCodeInfoPanel go)
            {
                go.transform.SetParent(gameObject.transform, true);
                go.gameObject.SetActive(false);
            });

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }

        Renewal();
    }

    public void ShowInfo()
    {
        // Clear InfoPanel
        foreach (var item in infoList) 
        {
            infoPool.Release(item);
        }
        infoList.Clear();

        var ct = CsvTableMgr.GetTable<CodeTable>().dataTable;
        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;
        

        foreach (var id in PlayDataManager.data.SkillCodes)
        {
            var go1 = infoPool.Get();
            go1.nameText.text = st[skt[ct[id].skill1_id].name];
            go1.levelText.text = $"Lv.{ct[id].skill1_lv}"; // 수정 요구
            infoList.Add(go1);

            if (ct[id].skill2_id != -1)
            {
                var go2 = infoPool.Get();
                go2.nameText.text = st[skt[ct[id].skill2_id].name];
                go2.levelText.text = $"Lv.{ct[id].skill2_lv}"; // 수정 요구
                infoList.Add(go2);
            }

        }

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

        // Left Lock
        var left = PlayDataManager.GetSocket() - PlayDataManager.data.SkillCodes.Count;
        for (int i = left; i > 0; i--)
        {
            var go = lockPool.Get();
            go.LockMode(false);

            lockList.Add(go);
        }
        // Full Lock
        for (int i = 15 - PlayDataManager.GetSocket(); i > 0; i--)
        {
            var go = lockPool.Get();
            go.LockMode();

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

    public void SortSkillCode(int codename)
    {
        
    }

    public void Renewal()
    {
        Clear();

        ShowInfo();
        ShowAll(); // test code
        ShowEquip();
    }
    
    private void OnEnable()
    {
        Renewal();
    }
}
