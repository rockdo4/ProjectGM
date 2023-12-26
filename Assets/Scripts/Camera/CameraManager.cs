using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<CameraManager>();
            }
            return m_instance;
        }
    }
    private static CameraManager m_instance;

    private CinemachineBasicMultiChannelPerlin noise;

    private CinemachineVirtualCamera[] playerCameras;
    private CinemachineVirtualCamera[] enemyCameras;
    private CinemachineVirtualCamera currentVirtualCamera;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        playerCameras = GameObject.FindWithTag(Tags.player)?.GetComponentsInChildren<CinemachineVirtualCamera>();
        enemyCameras = GameObject.FindWithTag(Tags.enemy)?.GetComponentsInChildren<CinemachineVirtualCamera>();

        currentVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        playerCameras = GameObject.FindWithTag(Tags.player)?.GetComponentsInChildren<CinemachineVirtualCamera>();
        enemyCameras = GameObject.FindWithTag(Tags.enemy)?.GetComponentsInChildren<CinemachineVirtualCamera>();
        SetCamera(Tags.player);
    }

    private void Update()
    {
        if (true)
        {

        }
    }

    public void SetCamera(string tag, int index = 0)
    {
        if (tag == Tags.player)
        {
            if (playerCameras.Length <= index)
            {
                return;
            }
            currentVirtualCamera = playerCameras[index];
        }
        else if (tag == Tags.enemy)
        {
            if (enemyCameras.Length <= index)
            {
                return;
            }
            currentVirtualCamera = enemyCameras[index];
        }

        currentVirtualCamera.enabled = true;    
    }

    public void Noise(float amplitudeGain, float frequencyGain, Vector3 pivotOffset = default)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
        if (pivotOffset != default)
        {
            noise.m_PivotOffset = pivotOffset;
        }
        noise.enabled = true;
    }

    public void StopNoise()
    {
        noise.enabled = false;
    }
}
