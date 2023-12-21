using UnityEngine;
using UnityEngine.UI;

public class LockerButton : MonoBehaviour
{
    [Header("잠금 이미지")]
    [SerializeField]
    private Image lockImage;

    public void LockMode(bool isLock = true)
    {
        lockImage.gameObject.SetActive(isLock);
    }
}
