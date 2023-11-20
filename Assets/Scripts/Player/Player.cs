using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddComponent<SwipeInput>();
        gameObject.AddComponent<TouchManager>();
    }

    private void Start()
    {
        SwipeInput.SwipeListeners += () =>
        {
            Debug.Log($"SwipeDirection: {SwipeInput.swipeDirection.ToString()}");
        };
    }
    private void Update()
    {
        if (TouchManager.IsHold)
        {
            Debug.Log("Holdddddddd");
        }
    }
}
