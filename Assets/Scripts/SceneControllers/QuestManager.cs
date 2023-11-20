using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private void Start()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
    }

}
