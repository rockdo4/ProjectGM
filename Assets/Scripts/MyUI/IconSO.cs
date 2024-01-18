using UnityEngine;

[CreateAssetMenu(menuName = "IconSO")]
public class IconSO : ScriptableObject
{
    [System.Serializable]
    public struct IconSOInfo
    {
        public int ID;
        public Sprite IMAGE;
    }

    [Header("ID / IMAGE")]
    [SerializeField]
    private IconSOInfo[] ARRAY;

    public Sprite GetSprite(int id)
    {
        for (int i = 0; i < ARRAY.Length; i++)
        {
            if (ARRAY[i].ID == id)
            {
                return ARRAY[i].IMAGE;
            }
        }
        return null;
    }
}
