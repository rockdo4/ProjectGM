using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "IconSO")]
public class IconSO : ScriptableObject
{
    [Header("재료 ID")]
    public int[] IDs;

    [Header("이미지")]
    public Sprite[] IMAGEs;

    public Sprite GetSprite(int id)
    {
        for (int i = 0; i < IDs.Length; i++)
        {
            if (IDs[i] == id)
            {
                return IMAGEs[i];
            }
        }
        return null;
    }
}
