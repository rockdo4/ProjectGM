
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class PlayerMovement : MonoBehaviour
    {
    public float speed = 5f;

    private void Update()
    {
        // WASD 키 입력을 받아 이동 벡터를 계산합니다.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;

        // 플레이어의 현재 위치
        Vector3 currentPosition = transform.position;

        // 새로운 위치 계산
        Vector3 newPosition = currentPosition + movement;

        // 이동 시 벽을 통과하지 못하도록 처리
        RaycastHit hit;
        if (Physics.Raycast(currentPosition, movement.normalized, out hit, movement.magnitude))
        {
            // 충돌이 발생하면 이동을 중지
            newPosition = hit.point;
        }

        // 새로운 위치로 이동
        transform.position = newPosition;
    }
}


