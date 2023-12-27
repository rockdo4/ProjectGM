using TMPro;
using UnityEngine;

public class SkillCodeInfoPanel : MonoBehaviour
{
    [Header("이름 텍스트")]
    public TextMeshProUGUI nameText;

    [Header("레벨 텍스트")]
    public TextMeshProUGUI levelText;

    private void OnEnable()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        transform.localScale = Vector3.one;
#endif
    }
}
