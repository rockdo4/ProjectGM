using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // 충돌한 객체가 "Player" 태그를 가진 경우 여기서 원하는 동작을 수행합니다.
                Debug.Log("Player collided with transparent wall!");
            }
        }
    }

