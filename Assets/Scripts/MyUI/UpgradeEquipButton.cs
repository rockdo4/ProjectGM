using UnityEngine;
using UnityEngine.UI;

public class UpgradeEquipButton : MonoBehaviour, IRenewal
{
    [Header("업그레이드 가능 이미지")]
    public Image upgradableImage;

    [Header("아이콘 이미지")]
    public Image iconImage;

    public Button button;

    private Equip item = null;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Renewal()
    {
        switch (item.type)
        {
            case Equip.EquipType.Weapon:

                break;

            case Equip.EquipType.Armor:

                break;
        }

        upgradableImage.color = (IsUpgradable()) ? Color.blue : Color.gray;
    }

    private bool IsUpgradable()
    {
        return false;
    }

    public void SetEquip(Equip item)
    {
        this.item = item;
    }
}