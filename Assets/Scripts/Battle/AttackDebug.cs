using UnityEngine;

public class AttackDebug : MonoBehaviour, IAttackable
{
    public void OnAttack(GameObject attacker, Attack attack)
    {
        Debug.Log(GetComponent<LivingObject>().HP);
        Debug.Log($"{attacker.name} attacked {gameObject.name} for {attack.Damage} damage. targetHP: {GetComponent<LivingObject>().HP}/{GetComponent<LivingObject>().stat.HP} Critical: {attack.IsCritical}");
    }
}
