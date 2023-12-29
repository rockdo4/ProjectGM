using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour, IRenewal
{
    [Header("마스터 믹서")]
    [SerializeField]
    private AudioMixer mixer;

    [Header("소지금 텍스트")]
    [SerializeField]
    private TextMeshProUGUI moneyText;

    [Header("진동기능")]
    [SerializeField]
    private Toggle vibeToggle;

    [Header("오디오 슬라이더")]
    [SerializeField]
    private Slider masterSlider;

    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Slider uiSlider;

    public static TitleManager Instance;

    private void Awake()
    {
        Instance = this;

        if (PlayDataManager.data == null)
        {
            PlayDataManager.Init();
        }
        vibeToggle.isOn = PlayDataManager.data.Vibration;

        Renewal();
    }

    private void Start()
    {
        mixer.SetFloat("masterVol", Mathf.Log10(PlayDataManager.data.masterVol) * 20);
        mixer.SetFloat("musicVol", Mathf.Log10(PlayDataManager.data.musicVol) * 20);
        mixer.SetFloat("sfxVol", Mathf.Log10(PlayDataManager.data.sfxVol) * 20);
        mixer.SetFloat("uiVol", Mathf.Log10(PlayDataManager.data.uiVol) * 20);

        masterSlider.value = PlayDataManager.data.masterVol;
        musicSlider.value = PlayDataManager.data.musicVol;
        sfxSlider.value = PlayDataManager.data.sfxVol;
        uiSlider.value = PlayDataManager.data.uiVol;
        
        ChangeMasterVol();
        ChangeMusicVol();
        ChangeSfxVol();
        ChangeUiVol();
    }

    public void ClearData()
    {
        PlayDataManager.Reset();
        MyNotice.Instance.Notice("데이터를 초기화 하였습니다.");
        moneyText.text = PlayDataManager.data.Gold.ToString();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Renewal();
        }
    }

    public void Renewal()
    {
        moneyText.text = PlayDataManager.data.Gold.ToString();
    }

    public void ChangeMasterVol()
    {
        mixer.SetFloat("masterVol", Mathf.Log10(masterSlider.value) * 20);
    }

    public void ChangeMusicVol()
    {
        mixer.SetFloat("musicVol", Mathf.Log10(musicSlider.value) * 20);
    }

    public void ChangeSfxVol()
    {
        mixer.SetFloat("sfxVol", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void ChangeUiVol()
    {
        mixer.SetFloat("uiVol", Mathf.Log10(uiSlider.value) * 20);
    }

    public void Save()
    {
        PlayDataManager.data.masterVol = masterSlider.value;
        PlayDataManager.data.musicVol = musicSlider.value;
        PlayDataManager.data.sfxVol = sfxSlider.value;
        PlayDataManager.data.uiVol = uiSlider.value;
        PlayDataManager.data.Vibration = vibeToggle.isOn;

        PlayDataManager.Save();
    }
}
