using TMPro;
using UnityEngine;

public class TakeAttackDebugEnemy : MonoBehaviour, IAttackable
{
    private EnemyAI enemy;

    private GameObject hpUI;
    private GameObject groggyUI;

    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        if (TestLogManager.Instance == null)
        {
            return;
        }
        hpUI = TestLogManager.Instance.MakeUI(HPAction);
        hpUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "<color=red>적 HP</color>";

        groggyUI = TestLogManager.Instance.MakeUI(GroggyAction);
        groggyUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "<color=red>적 그로기\n상태</color>";
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {

    }

    #region TestLog Events
    private void HPAction()
    {
        hpUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = enemy.HP switch
        {
            var x when x >= enemy.Stat.HP * 0.4f && x < enemy.Stat.HP * 0.7f => $"<color=yellow>{enemy.HP}</color>/{enemy.Stat.HP}",
            var x when x < enemy.Stat.HP * 0.4f => $"<color=red>{enemy.HP}</color>/{enemy.Stat.HP}",
            _ => $"<color=green>{enemy.HP}</color>/{enemy.Stat.HP}"
        };
    }

    private void GroggyAction()
    {
        groggyUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = enemy.IsGroggy switch
        {
            true => $"<color=green>ON</color>",
            false => $"<color=white>OFF</color>"
        };
    }
    #endregion
}
