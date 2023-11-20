using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    [Header("스와이프 시간")]
    [SerializeField]
    public float maxSwipeTime = 0.2f;

    [Header("스와이프 판정 비율")]
    [Range(0f, 1f)]
    public float minSwipeDistance = 0.2f;
    public enum SwipeDirection
    {
        None, Left, Right, Up, Down
    }
    public static SwipeDirection swipeDirection = SwipeDirection.None;

    public static bool Swiped { get; private set; }

    public delegate void OnSwipe();
    public static event OnSwipe SwipeListeners;

    Vector2 startPosition;
    float startTime;

    private void Update()
    {
        Swiped = false;
        swipeDirection = SwipeDirection.None;

        if (Input.touches.Length > 0)
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPosition = new Vector2(touch.position.x / screenSize.x, touch.position.y / screenSize.x);
                startTime = Time.time;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                if (Time.time - startTime > maxSwipeTime)
                {
                    return;
                }

                Vector2 endPosition = new Vector2(touch.position.x / screenSize.x, touch.position.y / screenSize.x);
                Vector2 swipe = new Vector2(endPosition.x - startPosition.x, endPosition.y - startPosition.y);

                if (swipe.magnitude < minSwipeDistance)
                {
                    return;
                }

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                { // Horizontal
                    if (swipe.x > 0)
                    {
                        Swiped = true;
                        swipeDirection = SwipeDirection.Right;
                    }
                    else
                    {
                        Swiped = true;
                        swipeDirection = SwipeDirection.Left;
                    }
                }
                else
                { // Vertical
                    if (swipe.y > 0)
                    {
                        Swiped = true;
                        swipeDirection = SwipeDirection.Up;
                    }
                    else
                    {
                        Swiped = true;
                        swipeDirection = SwipeDirection.Down;
                    }
                }
            }
        }

        if (Swiped)
        {
            if (SwipeListeners != null)
            {
                SwipeListeners();
            }
        }
    }
}
