using Unity.VisualScripting;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public static bool IsTap { get; private set; }
    public static bool IsHold {  get; private set; }

    [Header("홀드 판정 시간")]
    public float holdTime = 0.3f;
    private float holdTimer = 0f;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            IsTap = true;
        }

        if (IsTap)
        {
            Holding();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (holdTimer < holdTime)
            {
                Debug.Log("Tap");
            }

            holdTimer = 0f;
            IsTap = false;
            IsHold = false;
            return;
        }
    }

    private void Holding()
    {
        holdTimer += Time.deltaTime;
        if (holdTimer >= holdTime)
        {
            IsHold = true;
        }
    }
}