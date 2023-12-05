using UnityEngine;

public class WeaponPrefab : MonoBehaviour, IWear
{
    public Weapon item = null;

    public PlayerAnimationSO animationSO;

    [Header("무기 속성"), Tooltip("None: -, Hit: 타격, Slash: 참격, Pierce: 관통")]
    public AttackType type = AttackType.None;

    [Header("공격력")]
    public float attack;

    [Header("사거리")]
    public float attackRange = 2f;

    [Header("속성 배율")]
    public float weakDamage;
    
    public bool IsDualWield
    {
        get
        {
            if (item.weaponType == WeaponType.Tonpa)
            {
                return true;
            }
            return false;
        }
    }

    public void OnEquip(Weapon item)
    {
        type = item.attackType;

        var table = CsvTableMgr.GetTable<WeaponTable>().dataTable[(Weapon.WeaponID)item.id];
        attack = table.atk;
        weakDamage = table.weakpoint;
    }

    public void OnEquip(Weapon item, Animator anim)
    {
        OnEquip(item);

        anim.runtimeAnimatorController = animationSO.GetAnimator((Weapon.WeaponID)item.id);
    }
}
