using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatPanel : MonoBehaviour, IRenewal
{
    [Header("재료 이름")]
    [SerializeField]
    private TextMeshProUGUI nameText;

    [Header("아이콘 이미지")]
    public Image iconImage;

    [Header("판매 가격")]
    [SerializeField]
    private TextMeshProUGUI sellText;

    [Header("설명")]
    [SerializeField]
    private TextMeshProUGUI infoText;

    private Materials mat;

    public void SetMaterials(Materials mat)
    {
        this.mat = mat;

        // table
        nameText.text = mat.id.ToString();
    }

    public void Renewal()
    {
        gameObject.SetActive(true);


    }

}