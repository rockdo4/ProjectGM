using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player player { get; private set; }

    // equip weapon test
    private Item equipWeapon = null;
    public Transform leftHand;
    public Transform rightHand;
    public ItemSO weaponSO;

    private void Awake()
    {
        player = GetComponent<Player>();

        // equip weapon test
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        equipWeapon = PlayDataManager.data.Inventory.Find
            (i => i.instanceID == PlayDataManager.data.Equipment[Item.ItemType.Weapon]);
        

    }

    private void Start()
    {
        TouchManager.Instance.TapListeners += () =>
        {
            Debug.Log("Taped");
        };
        TouchManager.Instance.SwipeListeners += () =>
        {
            if (player.IsAttack)
            {
                return;
            }
            player.anim.Play("Idle");
            player.SetState(Player.States.Evade);
        };
        TouchManager.Instance.HoldListeners += () =>
        {
            if (player.currentState == Player.States.Evade)
            {
                return;
            }
            player.SetState(Player.States.Attack);
        };
        TouchManager.Instance.HoldEndListeners += () =>
        {
            Debug.Log("HoldEnd");
        };

        weaponSO.MakeItem(equipWeapon, rightHand, player.anim);
    }

    private void Update()
    {
        //피격 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.evadeTimer < player.stat.justEvadeTime)
            {
                player.evadePoint += player.stat.justEvadePoint;
            }
            else if (player.evadeTimer >= player.stat.justEvadeTime && player.evadeTimer < player.stat.evadeTime)
            {
                player.evadePoint += player.stat.evadePoint;
            }
            else
            {
                player.evadePoint += player.stat.hitEvadePoint;
            }
            player.evadePoint = Mathf.Clamp(player.evadePoint, (int)player.slider.minValue, (int)player.slider.maxValue);
            player.slider.value = player.evadePoint;
        }
    }

}