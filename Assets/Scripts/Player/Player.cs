using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
    }

    private void Start()
    {
        TouchManager.Instance.TapListeners += Attack;
        TouchManager.Instance.SwipeListeners += Evade;
        TouchManager.Instance.HoldListeners += AutoAttack;
    }

    private void Evade()
    {
        var swipeDirection = TouchManager.Instance.swipeDirection;
        Debug.Log($"Evade {swipeDirection}");
        switch (swipeDirection)
        {
            case TouchManager.SwipeDirection.None:
                break;
            case TouchManager.SwipeDirection.Left:
                break;
            case TouchManager.SwipeDirection.Right:
                break;
            case TouchManager.SwipeDirection.Up:
                break;
            case TouchManager.SwipeDirection.Down:
                break;
        }
    }

    private void Attack()
    {
        Debug.Log("Tap");
    }

    private void AutoAttack()
    {
        Debug.Log("Hold");
    }
}
