using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : Singleton<TouchManager>
{
    public enum SwipeDirection
    {
        None, Left, Right, Up, Down
    }
    public SwipeDirection SwipeDir { get; private set; } = SwipeDirection.None;
    public bool Taped { get; private set; }
    public bool Holded { get; private set; }
    public bool Swiped { get; private set; }

    [Header("홀드 판단 시간")]
    [SerializeField] private float holdTime = 0.02f;
    private float holdTimer = 0f;

    [Header("스와이프 판단 시간")]
    [SerializeField] private float swipeTime = 0.3f;

    [Header("스와이프 판정 거리")]
    [Range(0f, 5f)]
    [SerializeField] private float swipeDistance = 0.1f;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float startTime;

    #region Events
    public Action SwipeListeners;

    public Action TapListeners;

    public Action HoldListeners;

    public Action HoldEndListeners;
    #endregion

    private float screenDPI;
    private const float defaultDPI = 96;

    private bool isWork = true;

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
        if (!isWork)
        {
            return;
        }

        if (UICheck())
        {
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            endPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            startTime = Time.time;
            if (HoldListeners != null)
            {
                HoldListeners();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Swiped)
            {
                return;
            }

            if (!Holded)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTime)
                {
                    Holded = true;
                }
            }
            if (Holded && HoldListeners != null)
            {
                HoldListeners();
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
                    if (HoldListeners != null)
                    {
                        HoldListeners();
                    }
                }
                break;
            case TouchPhase.Moved:
                {
                    Holded = false;
                    if (Swiped)
                    {
                        return;
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
            case TouchPhase.Stationary:
                {
                    if (!Holded)
                    {
                        holdTimer += Time.deltaTime;
                        if (holdTimer >= holdTime)
                        {
                            Holded = true;
                        }
                    }
                    if (Holded && HoldListeners != null)
                    {
                        HoldListeners();
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
                SwipeDir = SwipeDirection.Right;
            }
            else
            {
                SwipeDir = SwipeDirection.Left;
            }
        }
        else
        { // Vertical
            if (swipe.y > 0)
            {
                SwipeDir = SwipeDirection.Up;
            }
            else
            {
                SwipeDir = SwipeDirection.Down;
            }
        }
        Swiped = true;
    }
    public bool UICheck()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            return false;
        }

        var isUI = false;
#if UNITY_EDITOR || UNITY_STANDALONE
        isUI = eventSystem.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
        isUI = Input.touchCount > 0 && eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        if (isUI)
        {
            var ui = eventSystem.currentSelectedGameObject;
            if (ui != null && ui.tag != Tags.ignoreUI)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDisable()
    {
        isWork = false;
    }
    private void OnEnable()
    {
        isWork = true;
    }
}