using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField]
    private Player player;

    
    [Header("스킬 SO")]
    [SerializeField]
    private SkillSO skillSO;
    

    private void Awake()
    {
        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }    

        foreach (var item in PlayDataManager.curSkill)
        {
            skillSO.GetSkill(item.Key, item.Value);
        }
    }
}
