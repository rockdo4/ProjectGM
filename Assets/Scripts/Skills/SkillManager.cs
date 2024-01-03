using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("½ºÅ³ SO")]
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
            var skill = skillSO.GetSkill(item.Key, item.Value);
            skill.transform.SetParent(transform, false);
        }

        var setinfo = PlayDataManager.curSetSkill;
        {
            var skill = skillSO.GetSkill(setinfo.id, setinfo.level);
            skill.transform.SetParent(transform, false);
        }

    }
}
