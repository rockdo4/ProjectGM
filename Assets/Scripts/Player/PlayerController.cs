using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player2 player { get; private set; }

    // equip weapon test
    private Item equipWeapon = null; 
    public Transform hand;
    public ItemSO weaponSO;

    private void Awake()
    {
        player = GetComponent<Player2>();

        // equip weapon test
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        equipWeapon = PlayDataManager.data.Inventory.Find
            (i => i.instanceID == PlayDataManager.data.Equipment[Item.ItemType.Weapon]);
        weaponSO.MakeItem(equipWeapon, hand);
    }

    private void Start()
    {
        TouchManager.Instance.TapListeners += () =>
        {
            player.anim.SetTrigger("Attack"); // animation test code
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