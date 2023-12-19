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
    [SerializeField] private float holdTime = 0.01f;
    private float holdTimer = 0f;

    [Header("스와이프 판단 시간")]
    [SerializeField] private float swipeTime = 0.3f;

    [Header("스와이프 판정 거리")]
    [Range(0f, 5f)]
    [SerializeField] private float swipeDistance = 0.5f;

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

    private float screenDPI;
    private const float defaultDPI = 96;

    private void Awake()
    {
        screenDPI = Screen.dpi;
        if (screenDPI == 0)
        {
            screenDPI = defaultDPI;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = new Vector2(Input.mousePosition.x , Input.mousePosition.y);
            endPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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
                endPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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

            if (TapListeners != null)
            {
                TapListeners();
            }
            return;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount < 1)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                {
                    startPosition = new Vector2(touch.position.x, touch.position.y);
                    endPosition = new Vector2(touch.position.x, touch.position.y);
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
                        endPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        SwipeDetected();

                        if (Swiped && SwipeListeners != null)
                        {
                            SwipeListeners();
                        }
                    }
                }
                break;
            case TouchPhase.Ended:
                {
                    Taped = Holded = Swiped = false;
                    holdTimer = 0f;

                    if (HoldEndListeners != null)
                    {
                        HoldEndListeners();
                    }
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
        swipe /= screenDPI;
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