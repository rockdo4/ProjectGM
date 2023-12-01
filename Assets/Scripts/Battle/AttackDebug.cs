using TMPro;
using UnityEngine;

public class AttackDebug : MonoBehaviour, IAttackable
{
    private enum EvadeSuccesss
    {
        None, Normal, Just
    }
    private EvadeSuccesss evade;
    private Player player;
    private GameObject debugUI;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        //Debug.Log($"{attacker.name} attacked {gameObject.name} for {attack.Damage} damage.\n{gameObject.name}_HP: {GetComponent<LivingObject>().HP}/{GetComponent<LivingObject>().stat.HP}\tIsCritical: {attack.IsCritical}");
    }
}
