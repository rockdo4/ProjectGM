using TMPro;
using UnityEngine;
using static Player;

public class PlayerTestLog : MonoBehaviour
{
    private Player player;

    private GameObject hpUI;
    private GameObject evadeUI;
    private GameObject evadePointUI;
    private GameObject groggyAttackUI;
    private GameObject stateUI;
    private GameObject attackUI;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        if (TestLogManager.Instance == null)
        {
            return;
        }
        hpUI = TestLogManager.Instance.MakeUI(HPAction);
        hpUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "플레이어HP";

        evadeUI = TestLogManager.Instance.MakeUI(EvadeAction);
        evadeUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "회피";

        evadePointUI = TestLogManager.Instance.MakeUI();
        evadePointUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "회피 포인트\n그로기 공격";

        groggyAttackUI = TestLogManager.Instance.MakeUI(GroggyAction);
        groggyAttackUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "특수공격";

        stateUI = TestLogManager.Instance.MakeUI(StateAction);
        stateUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "상태";

        attackUI = TestLogManager.Instance.MakeUI(AttackAction);
        attackUI.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "공격 상세";
    }

    #region TestLog Events
    private void HPAction()
    {
        hpUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = player.HP switch
        {
            var x when x >= player.Stat.HP * 0.4f && x < player.Stat.HP * 0.7f => $"<color=yellow>{player.HP}</color>/{player.Stat.HP}",
            var x when x < player.Stat.HP * 0.4f => $"<color=red>{player.HP}</color>/{player.Stat.HP}",
            _ => $"<color=green>{player.HP}</color>/{player.Stat.HP}"
        };
    }

    private void EvadeAction()
    {
        evadeUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = player.evadeTimer switch
        {
            float x when (player.GetComponent<PlayerController>().currentState == PlayerController.State.Evade && x < player.Stat.justEvadeTime) => "<color=green>저스트 회피</color>",
            float x when (player.GetComponent<PlayerController>().currentState == PlayerController.State.Evade && x >= player.Stat.justEvadeTime && x < player.Stat.evadeTime) => "<color=yellow>일반 회피</color>",
            _ => "<color=red>통상</color>"
        };
        evadePointUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = player.GroggyAttack switch
        {
            true => $"<color=yellow>{(int)player.evadePoint}</color>/{player.Stat.maxEvadePoint}\n<color=green>그로기 공격 ON</color>",
            false => $"<color=blue>{(int)player.evadePoint}</color>/{player.Stat.maxEvadePoint}\n<color=white>그로기 공격 OFF</color>"
        };
    }
    private void GroggyAction()
    {
        groggyAttackUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = player.Enemy.IsGroggy switch
        {
            true => $"<color=green>ON</color>",
            false => $"<color=white>OFF</color>"
        };
    }
    private void StateAction()
    {
        stateUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = player.GetComponent<PlayerController>().currentState switch
        {
            PlayerController.State.Death => $"<color=red>죽음</color>",
            PlayerController.State.Hit => $"<color=red>피격</color>",
            PlayerController.State.Attack => $"<color=yellow>공격==>>></color>",
            PlayerController.State.Evade => $"<color=green>회피</color>",
            PlayerController.State.Sprint => $"<color=orange>전진</color>",
            PlayerController.State.SuperAttack => $"<color=blue>특수공격</color>",
            PlayerController.State.Idle => $"<color=white>일반</color>",
            _ => $"<color=white>일반</color>"
        };
    }

    private void AttackAction()
    {
        if (player.GetComponent<PlayerController>().currentState != PlayerController.State.Attack)
        {
            attackUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = $"<color=white>공격 아님</color>";
            return;
        }
        attackUI.GetComponentsInChildren<TextMeshProUGUI>()[1].text = player.attackState switch
        {
            AttackState.Before => $"<color=yellow>선딜레이</color>\n<color=green>회피 가능</color>",
            AttackState.Attack => $"<color=yellow>공격</color>\n<color=red>회피 불가</color>",
            AttackState.AfterStart => $"<color=yellow>후딜레이 시작</color>\n<color=red>회피 불가</color>",
            AttackState.AfterEnd => $"<color=yellow>후딜레이 종료</color>\n<color=green>회피 가능</color>",
            AttackState.End => $"<color=yellow>공격 종료</color>\n<color=green>회피 가능</color>",
            _ => $"<color=yellow></color>"
        };
    }
    #endregion
}
