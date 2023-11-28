using UnityEngine;

public class TakeAttack : MonoBehaviour, IAttackable
{
    private Stat stat;
    private LivingObject livingObject;

    private void Awake()
    {
        //stat = GetComponent<ILivingObject>().GetStat();
        stat = GetComponent<LivingObject>().stat;
        livingObject = GetComponent<LivingObject>();
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        Debug.Log($"{attacker.name} / {attack.Damage}");
        Debug.Log(livingObject.HP);

        if (stat == null || attacker == null)
        {
            return;
        }
        livingObject.HP -= attack.Damage;
        if (livingObject.HP <= 0)
        {
            livingObject.HP = 0;
        }
        //stat.HP -= attack.Damage;
        //if (stat.HP <= 0)
        //{
        //    stat.HP = 0;
        //}
        
    }
}
