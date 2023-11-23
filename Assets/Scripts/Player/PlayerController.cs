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
        //피격 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.evadeTimer < player.stat.justEvadeTime)
            {
                player.ren.material.color = player.justEvadeSuccessColor;
                player.evadePoint += player.stat.justEvadePoint;
            }
            else if (player.evadeTimer >= player.stat.justEvadeTime && player.evadeTimer < player.stat.evadeTime)
            {
                player.ren.material.color = player.evadeSuccessColor;
                player.evadePoint += player.stat.evadePoint;
            }
            else
            {
                player.ren.material.color = player.hitColor;
                player.evadePoint += player.stat.hitEvadePoint;
            }
            player.slider.value = player.evadePoint;
        }
    }
}