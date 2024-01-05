using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

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

    [Header("확장 Content")]
    [SerializeField]
    private GameObject exContent;

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

    [Header("정렬 Dropdown")]
    [SerializeField]
    private TMP_Dropdown dropdown;

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
                button.transform.SetParent(invContent.transform, false);
            },
            delegate (ItemButton button) // actionOnRelease
            {
                button.Clear();
                button.transform.SetParent(gameObject.transform, false); // ItemButton Transform Reset
                button.gameObject.SetActive(false);
            });

        lockPool = new ObjectPool<LockerButton>
            (() => // createFunc
            {
                var go = Instantiate(lockPrefab, equipContent.transform, false);
                go.gameObject.SetActive(false);
                return go;
            },
            delegate (LockerButton go) // actionOnGet
            {
                go.gameObject.SetActive(true);
                go.transform.SetParent(equipContent.transform, false);
            },
            delegate (LockerButton go) // actionOnRelease
            {
                go.transform.SetParent(gameObject.transform, false);
                go.gameObject.SetActive(false);
            });

        infoPool = new ObjectPool<SkillCodeInfoPanel>
            (() => // createFunc
            {
                var go = Instantiate(infoPrefab, infoContent.transform, false);
                go.gameObject.SetActive(false);
                return go;
            },
            delegate (SkillCodeInfoPanel go)
            {
                go.gameObject.SetActive(true);
                go.transform.SetParent(infoContent.transform, false);
            },
            delegate (SkillCodeInfoPanel go)
            {
                go.transform.SetParent(gameObject.transform, false);
                go.gameObject.SetActive(false);
            });

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        DropdownInit();

        Renewal();
    }

    private void DropdownInit()
    {
        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        foreach (var item in skt) 
        {
            if (item.Value.type != 1)
            {
                continue;
            }
            dropdown.options.Add(new TMP_Dropdown.OptionData(st[item.Value.name]));
        }
    }

    public void ShowInfo()
    {
        // Clear InfoPanel
        foreach (var item in infoList) 
        {
            infoPool.Release(item);
        }
        infoList.Clear();

        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        foreach (var skill in PlayDataManager.curSkill)
        {
            var go = infoPool.Get();
            go.nameText.text = st[skt[skill.Key].name];
            go.levelText.text = $"Lv.{skill.Value}"; // 수정 요구
            infoList.Add(go);

        }

    }

    public void ExpandInfo()
    {
        // Clear
        var infos = exContent.GetComponentsInChildren<SkillCodeInfoPanel>();
        foreach (var info in infos)
        {
            infoPool.Release(info);
        }

        foreach (var info in infoList)
        {
            var go = infoPool.Get();
            go.nameText.text = info.nameText.text;
            go.levelText.text = info.levelText.text;
            go.transform.SetParent(exContent.transform, false);
        }
    }

    public void ShowAll()
    {
        ClearItemButton();
        ShowEquip();

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
            go.transform.SetParent(equipContent.transform, false);
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
        if (codename == 0)
        {
            ShowAll();
            return;
        }

        ClearItemButton();
        ShowEquip();

        var skt = CsvTableMgr.GetTable<SkillTable>().dataTable;
        var ct = CsvTableMgr.GetTable<CodeTable>().dataTable;
        var st = CsvTableMgr.GetTable<StringTable>().dataTable;

        var str = st.FirstOrDefault(x => x.Value == dropdown.options[codename].text).Key;
        var target = skt.FirstOrDefault(x => x.Value.name == str).Key;

        foreach (var code in PlayDataManager.data.CodeInventory)
        {
            if (ct[code.id].skill1_id != target && ct[code.id].skill2_id != target)
            {
                continue;
            }
            var go = buttonPool.Get();

            go.iconImage.sprite = skillcodeIconSO.GetSprite(ct[code.id].type);
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

    public void Renewal()
    {
        Clear();

        ShowInfo();
        SortSkillCode(dropdown.value);
    }
    
    private void OnEnable()
    {
        Renewal();
    }
}
