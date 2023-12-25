using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class InventoryManager : MonoBehaviour, IRenewal
{
    public enum ItemType
    {
        Weapon,
        Armor,
        SkillCode,
        Mat
    }
    private ItemType curType = ItemType.Weapon;
    public static InventoryManager Instance;

    public GameObject inventoryPanel;

    public ItemButton buttonPrefab;

    [Header("무기")]
    public ItemPanel weaponPanel;
    public IconSO weaponIconSO;

    [Space(10.0f)]

    [Header("방어구")]
    public ItemPanel armorPanel;
    public IconSO armorIconSO;

    [Space(10.0f)]

    [Header("스킬코드")]
    public SkillCodePanel skillCodePanel;
    public IconSO skillIconSO;

    [Space(10.0f)]

    [Header("재료")]
    public MatPanel matPanel;
    public IconSO matIconSo;

    [Space(10.0f)]

    [Header("일괄판매")]
    public GameObject sellArea;
    public GameObject sellPanel;
    public GameObject sellButton;

    [Space(10.0f)]

    private bool sellMode = false;
    private List<Equip> sellEquipList = new List<Equip>();
    private List<SkillCode> sellSkillCodeList = new List<SkillCode>();
    private List<Materials> sellMatList = new List<Materials>();

    private ObjectPool<ItemButton> buttonPool;
    private List<ItemButton> releaseList = new List<ItemButton>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buttonPool = new ObjectPool<ItemButton>(
            () => // createFunc
        {
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(inventoryPanel.transform, true);
            button.OnCountAct();
            button.gameObject.SetActive(false);

            return button;
        },
        delegate (ItemButton button) // actionOnGet
        {
            button.gameObject.SetActive(true);
            button.transform.SetParent(inventoryPanel.transform, true);
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

        //TestAddItem();
        Renewal();
    }

    public void ShowWeapons(bool isOn = true)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.Weapon;

        var weapons = PlayDataManager.data.WeaponInventory;
        foreach (var weapon in weapons)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = weaponIconSO.GetSprite(weapon.id / 100 * 100 + 1);
            // weapon icon level reset

            go.iconImage.color = Color.white;
            go.OnEquip(weapon.isEquip);

            go.button.onClick.AddListener(() => 
            {
                if (sellMode)
                {
                    if (sellEquipList.Count >= 10) // Over Count Exception
                    {
                        MyNotice.Instance.Notice("최대 판매 개수를 넘길 수 없습니다.");
                        return;
                    }

                    if (weapon.isEquip) // Were Equip Exception
                    {
                        // Notice
                        MyNotice.Instance.Notice("판매할 수 없습니다.");
                        return;
                    }

                    if (go.iconImage.color == Color.white)
                    {
                        sellEquipList.Add(weapon);
                        go.iconImage.color = Color.red;

                        var newGo = buttonPool.Get();
                        newGo.iconImage.sprite = weaponIconSO.GetSprite(weapon.id / 100 * 100 + 1);
                        // weapon icon level reset

                        newGo.OnEquip(weapon.isEquip);
                        newGo.transform.SetParent(sellPanel.transform, true);
                        newGo.button.onClick.AddListener(() => 
                        {
                            buttonPool.Release(go.sell);
                            go.sell = null;
                            sellEquipList.Remove(weapon);
                            go.iconImage.color = Color.white;
                        });
                        releaseList.Add(newGo);

                        go.sell = newGo;
                    }
                    else
                    {
                        buttonPool.Release(go.sell);
                        go.sell = null;
                        sellEquipList.Remove(weapon);
                        go.iconImage.color = Color.white;
                    }
                }
                else
                {
                    weaponPanel.SetItem(weapon);
                    weaponPanel.iconImage.sprite = go.iconImage.sprite;
                    weaponPanel.Renewal();
                }
                
            });

            releaseList.Add(go);
        }
    }

    public void ShowArmors(bool isOn = true)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.Armor;

        var armors = PlayDataManager.data.ArmorInventory;
        foreach (var armor in armors)
        {
            var go = buttonPool.Get();

            go.iconImage.sprite = armorIconSO.GetSprite(armor.id / 100 * 100 + 1);
            go.iconImage.color = Color.white;
            go.OnEquip(armor.isEquip);

            go.button.onClick.AddListener(() =>
            {
                if (sellMode)
                {
                    if (sellEquipList.Count >= 10) // Over Count Exception
                    {
                        MyNotice.Instance.Notice("최대 판매 개수를 넘길 수 없습니다.");
                        return;
                    }

                    if (armor.isEquip) // Were Equip Exception
                    {
                        // Notice
                        MyNotice.Instance.Notice("판매할 수 없습니다.");
                        return;
                    }

                    if (go.iconImage.color == Color.white)
                    {
                        sellEquipList.Add(armor);
                        go.iconImage.color = Color.red;

                        var newGo = buttonPool.Get();
                        newGo.iconImage.sprite = weaponIconSO.GetSprite(armor.id / 100 * 100 + 1);
                        // armor icon level reset

                        newGo.OnEquip(armor.isEquip);
                        newGo.transform.SetParent(sellPanel.transform, true);
                        newGo.button.onClick.AddListener(() =>
                        {
                            buttonPool.Release(go.sell);
                            go.sell = null;
                            sellEquipList.Remove(armor);
                            go.iconImage.color = Color.white;
                        });
                        releaseList.Add(newGo);

                        go.sell = newGo;
                    }
                    else
                    {
                        buttonPool.Release(go.sell);
                        go.sell = null;
                        sellEquipList.Remove(armor);
                        go.iconImage.color = Color.white;
                    }
                }
                else
                {
                    armorPanel.SetItem(armor);
                    armorPanel.iconImage.sprite = go.iconImage.sprite;
                    armorPanel.Renewal();
                }
                
            });

            releaseList.Add(go);
        }
    }

    public void ShowSkillCodes(bool isOn = true)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.SkillCode;

        var skillcodes = PlayDataManager.data.CodeInventory;
        foreach (var skillcode in skillcodes) 
        {
            var go = buttonPool.Get();
            var table = CsvTableMgr.GetTable<CodeTable>().dataTable;

            go.transform.SetParent(inventoryPanel.transform, true);

            go.OnCountAct(true, skillcode.count);
            go.iconImage.sprite = skillIconSO.GetSprite(table[skillcode.id].type);
            go.OnEquip();

            go.GetComponent<ItemButton>().button.onClick.AddListener(() =>
            {
                if (sellMode)
                {
                    if (sellSkillCodeList.Count >= 10)
                    {
                        MyNotice.Instance.Notice("최대 판매 개수를 넘길 수 없습니다.");
                        return;
                    }

                    if (go.iconImage.color == Color.white)
                    {
                        sellSkillCodeList.Add(skillcode);
                        go.iconImage.color = Color.red;

                        var table = CsvTableMgr.GetTable<CodeTable>().dataTable;

                        var newGo = buttonPool.Get();
                        newGo.iconImage.sprite = skillIconSO.GetSprite(table[skillcode.id].type);
                        newGo.transform.SetParent(sellPanel.transform, true);
                        newGo.button.onClick.AddListener(() =>
                        {
                            buttonPool.Release(go.sell);
                            go.sell = null;
                            sellSkillCodeList.Remove(skillcode);
                            go.iconImage.color = Color.white;
                        });
                        releaseList.Add(newGo);

                        go.sell = newGo;
                    }
                    else
                    {
                        buttonPool.Release(go.sell);
                        go.sell = null;
                        sellSkillCodeList.Remove(skillcode);
                        go.iconImage.color = Color.white;
                    }
                }
                else
                {
                    skillCodePanel.SetSkillCode(skillcode);
                    skillCodePanel.iconImage.sprite = go.iconImage.sprite;
                    skillCodePanel.Renewal();
                }
            });

            releaseList.Add(go);
        }
    }

    public void ShowMaterials(bool isOn = true)
    {
        if (!isOn)
        {
            return;
        }
        ClearItemButton();
        curType = ItemType.Mat;

        var mats = PlayDataManager.data.MatInventory;
        foreach (var mat in mats)
        {
            var go = buttonPool.Get();
            go.transform.SetParent(inventoryPanel.transform);

            go.OnCountAct(true, mat.count);
            go.iconImage.sprite = matIconSo.GetSprite(mat.id);
            go.OnEquip();

            go.GetComponent<ItemButton>().button.onClick.AddListener(() =>
            {
                if (sellMode)
                {
                    if (sellMatList.Count >= 10)
                    {
                        MyNotice.Instance.Notice("최대 판매 개수를 넘길 수 없습니다.");
                        return;
                    }

                    if (go.iconImage.color == Color.white)
                    {
                        sellMatList.Add(mat);
                        go.iconImage.color = Color.red;

                        var newGo = buttonPool.Get();
                        newGo.iconImage.sprite = matIconSo.GetSprite(mat.id);
                        newGo.transform.SetParent(sellPanel.transform);
                        newGo.button.onClick.AddListener(() =>
                        {
                            buttonPool.Release(go.sell);
                            go.sell = null;
                            sellMatList.Remove(mat);
                            go.iconImage.color = Color.white;
                        });
                        releaseList.Add(newGo);

                        go.sell = newGo;
                    }
                    else
                    {
                        buttonPool.Release(go.sell);
                        go.sell = null;
                        sellMatList.Remove(mat);
                        go.iconImage.color = Color.white;
                    }
                }
                else
                {
                    matPanel.SetMaterials(mat);
                    matPanel.iconImage.sprite = go.iconImage.sprite;
                    matPanel.Renewal();
                }
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

        releaseList.Clear();
    }

    public void SellMode(bool mode)
    {
        sellMode = mode;
        sellArea.SetActive(mode);
        sellButton.SetActive(!mode);

        switch (curType)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
                sellEquipList.Clear();
                break;

            case ItemType.SkillCode:
                sellSkillCodeList.Clear();
                break;

            case ItemType.Mat:
                sellMatList.Clear();
                break;
        }

        switch (sellMode)
        {
            case true:

                break;

            case false:
                foreach (var item in releaseList)
                {
                    if (item.sell != null)
                    {
                        buttonPool.Release(item.sell);
                        item.sell = null;

                    }
                    item.iconImage.color = Color.white;
                }
                break;
        }
    }

    public void SellItem()
    {
        switch (curType)
        {
            case ItemType.Weapon:
                {
                    foreach (var item in sellEquipList)
                    {
                        PlayDataManager.SellItem(item as Weapon);
                    }
                    ShowWeapons();
                }
                break;

            case ItemType.Armor:
                {
                    foreach (var item in sellEquipList)
                    {
                        PlayDataManager.SellItem(item as Armor);
                    }
                    ShowArmors();
                }
                break;

            case ItemType.SkillCode:
                {
                    foreach (var item in sellSkillCodeList)
                    {
                        PlayDataManager.SellItem(item);
                    }
                    ShowSkillCodes();
                }
                break;

            case ItemType.Mat:
                {
                    foreach (var item in sellMatList)
                    {
                        PlayDataManager.SellItem(item);
                    }
                    ShowMaterials();
                }
                break;
        }
        TitleManager.Instance.Renewal();

        SellMode(false);
    }

    public void Renewal()
    {
        switch (curType)
        {
            case ItemType.Weapon:
                ShowWeapons();
                break;

            case ItemType.Armor:
                ShowArmors();
                break;

            case ItemType.SkillCode:
                ShowSkillCodes();
                break;

            case ItemType.Mat:
                ShowMaterials();
                break;
        }

        TitleManager.Instance.Renewal();
    }

    public void Tester()
    {
        var mt = CsvTableMgr.GetTable<MatTable>().dataTable;
        foreach (var mat in mt)
        {
            PlayDataManager.IncreaseMat(mat.Key, 99);
        }

        var sct = CsvTableMgr.GetTable<CodeTable>().dataTable;
        foreach (var code in sct)
        {
            PlayDataManager.IncreaseCode(code.Key, 99);
        }

        PlayDataManager.AddGold(100000);

        TitleManager.Instance.Renewal();
    }
}
