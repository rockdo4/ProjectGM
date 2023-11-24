using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    public enum SwipeDirection
    {
        None, Left, Right, Up, Down
    }
    public SwipeDirection swipeDirection = SwipeDirection.None;
    public bool Taped { get; private set; }
    public bool Holded { get; private set; }
    public bool Swiped { get; private set; }

    [Header("홀드 판단 시간")]
    public float holdTime = 0.02f;
    private float holdTimer = 0f;

    [Header("스와이프 판단 시간")]
    [SerializeField]
    public float swipeTime = 0.3f;

    [Header("스와이프 판정 거리")]
    [Range(0f, 1f)]
    public float swipeDistance = 0.15f;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float startTime;

    #region Events
    public delegate void OnSwipe();
    public event OnSwipe SwipeListeners;

    public delegate void OnTap();
    public event OnTap TapListeners;

    public delegate void OnHold();
    public event OnHold HoldListeners;

    public delegate void HoldEnd();
    public event HoldEnd HoldEndListeners;
    #endregion

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            endPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            startTime = Time.time;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Swiped)
            {
                return;
            }

            if (Holded && HoldListeners != null)
            {
                HoldListeners();
            }

            if (!Holded)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTime)
                {
                    Holded = true;
                }
            }

            if (!Swiped)
            {
                endPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
                SwipeDetected();

                if (Swiped && SwipeListeners != null)
                {
                    SwipeListeners();
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            Taped = Holded = Swiped = false;
            holdTimer = 0f;

            if (HoldEndListeners != null)
            {
                HoldEndListeners();
            }
            return;
            endPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            SwipeDetected();

            if (!Holded && !Swiped)
            {
                Taped = true;
            }

            if (Swiped && SwipeListeners != null)
            {
                Swiped = false;
                SwipeListeners();
            }
            else if (Taped && TapListeners != null)
            {
                Taped = false;
                TapListeners();
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount < 1)
        {
            return;
        }

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                {
                    startPosition = new Vector2(touch.position.x / screenSize.x, touch.position.y / screenSize.x);
                    endPosition = new Vector2(touch.position.x / screenSize.x, touch.position.y / screenSize.x);
                    startTime = Time.time;
                }
                break;
            case TouchPhase.Moved:
                {
                    Holded = false;
                }
                break;
            case TouchPhase.Stationary:
                {
                    if (Holded && HoldListeners != null)
                    {
                        HoldListeners();
                    }

                    if (!Holded)
                    {
                        holdTimer += Time.deltaTime;
                        if (holdTimer >= holdTime)
                        {
                            Holded = true;
                        }
                    }
                }
                break;
            case TouchPhase.Ended:
                {
                    endPosition = new Vector2(touch.position.x / screenSize.x, touch.position.y / screenSize.x);
                    SwipeDetected();

                    if (!Holded && !Swiped)
                    {
                        Taped = true;
                    }

                    if (Swiped && SwipeListeners != null)
                    {
                        Swiped = false;
                        SwipeListeners();
                    }
                    else if (Taped && TapListeners != null)
                    {
                        Taped = false;
                        TapListeners();
                    }

                    Taped = Holded = Swiped = false;
                    holdTimer = 0f;
                }
                break;
        }
#endif
    }

    public void SwipeDetected()
    {
        if (Time.time - startTime > swipeTime)
        {
            return;
        }
        Vector2 swipe = new Vector2(endPosition.x - startPosition.x, endPosition.y - startPosition.y);
        if (swipe.magnitude < swipeDistance)
        {
            return;
        }

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        { // Horizontal
            if (swipe.x > 0)
            {
                swipeDirection = SwipeDirection.Right;
            }
            else
            {
                swipeDirection = SwipeDirection.Left;
            }
        }
        else
        { // Vertical
            if (swipe.y > 0)
            {
                swipeDirection = SwipeDirection.Up;
            }
            else
            {
                swipeDirection = SwipeDirection.Down;
            }
        }
        Swiped = true;
    }
}