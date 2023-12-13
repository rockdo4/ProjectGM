using UnityEngine;

[CreateAssetMenu(menuName = "IconSO")]
public class IconSO : ScriptableObject
{
    [Header("재료 ID")]
    public int[] ID;

    [Header("이미지")]
    public Sprite[] IMAGE;

    public Sprite GetSprite(int id)
    {
        for (int i = 0; i < ID.Length; i++)
        {
            if (ID[i] == id)
            {
                return IMAGE[i];
            }
        }
        return null;
    }
}
