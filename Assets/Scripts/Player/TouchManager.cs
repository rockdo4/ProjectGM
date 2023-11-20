using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    public bool IsHold = false;
    private float holdTime = 0.3f;
    private float holdTimer = 0f;
    private float swipeTime = 0.1f;
    private float swipeTimer = 0f;
    private float swipeDistance = 0.2f;

    public Vector3 startPosition;
    public Vector3 endPosition;
    public float TouchDistance
    {
        get
        {
            return Vector3.Distance(startPosition, endPosition);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //startPosition = Input.GetTouch(0).position;
            startPosition = Input.mousePosition;
            Holding();
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPosition = Input.mousePosition;
        }

        if (Input.touchCount == 0)
        {
            startPosition = Vector3.zero;
            endPosition = Vector3.zero;
            return;
        }
    }

    private void Holding()
    {
        if (!Input.GetMouseButtonUp(0))
        {
            holdTimer = 0f;
            IsHold = false;
            return;
        }

        holdTimer += Time.deltaTime;
        if (holdTimer > holdTime)
        {
            IsHold = true;
        }
    }
}