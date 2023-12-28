using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Player player;
    
    [Header("½ºÅ³ SO")]
    [SerializeField]
    private SkillSO skillSO;
    
    private void Awake()
    {
        player = GetComponent<Player>();

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }    

        foreach (var item in PlayDataManager.curSkill)
        {
            var skill = skillSO.GetSkill(item.Key, item.Value);
            skill.transform.SetParent(transform);
        }

    }
}
