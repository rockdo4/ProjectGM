using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateArmorPanel : MonoBehaviour, IRenewal
{

    public void Renewal()
    {
        gameObject.SetActive(true);
    }
}
