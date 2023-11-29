// AttackCell.cs
using UnityEngine;

public class AttackCell : MonoBehaviour
{
    public bool playerInside = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("온트리거엔터");

            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("온트리거Exit");

            playerInside = false;
        }
    }
}
