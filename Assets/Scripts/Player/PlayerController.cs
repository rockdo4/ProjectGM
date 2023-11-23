using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player2 player { get; private set; }

    private void Awake()
    {
        player = GetComponent<Player2>();
    }

    private void Start()
    {
        TouchManager.Instance.TapListeners += () =>
        {
            player.SetState(Player2.States.Idle);
        };
        TouchManager.Instance.SwipeListeners += () =>
        {
            if (player.currentState == Player2.States.Evade)
            {
                return;
            }
            player.SetState(Player2.States.Evade);
        };
        TouchManager.Instance.HoldListeners += () =>
        {

        };
    }

    private void Update()
    {
        player.rigid.transform.LookAt(player.enemy.transform);
    }
}