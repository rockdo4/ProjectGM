using UnityEngine;

public class WeaponPrefab : MonoBehaviour, IEquip
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

    public void OnEquip()
    {
        // Define AttackType
        type = item.attackType;
    }

    public void OnEquip(Item item)
    {
        this.item = item as Weapon;
        OnEquip();
    }

    public void OnEquip(Item item, Animator anim)
    {
        OnEquip(item);

        anim.runtimeAnimatorController = animationSO.GetAnimator((Weapon.WeaponID)item.id);
    }
}
