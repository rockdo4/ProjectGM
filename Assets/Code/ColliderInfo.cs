using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColliderInfo : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        // 캡슐 콜라이더를 가져옴
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (capsuleCollider == null)
        {
            Debug.LogError("Capsule Collider not found on the object.");
        }

        // 콜라이더 정보 출력
        DisplayColliderInfo();
    }

    private void DisplayColliderInfo()
    {
        if (capsuleCollider != null)
        {
            Debug.Log("Capsule Collider Info:");
            Debug.Log($"Height: {capsuleCollider.height}");
            Debug.Log($"Radius: {capsuleCollider.radius}");
            Debug.Log($"Center: {capsuleCollider.center}");
        }
    }
}


