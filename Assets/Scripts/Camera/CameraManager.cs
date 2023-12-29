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
        SetCameraWithTag(Tags.player);
    }

    public void SetCameraWithTag(string tag, int index = 0)
    {
        if (playerCameras == null)
        {
            playerCameras = GameObject.FindWithTag(Tags.player)?.GetComponentsInChildren<CinemachineVirtualCamera>();
        }
        if (enemyCameras == null)
        {
            enemyCameras = GameObject.FindWithTag(Tags.enemy)?.GetComponentsInChildren<CinemachineVirtualCamera>();
        }
        currentVirtualCamera?.gameObject.SetActive(false);
        if (tag == Tags.player)
        {
            for(int i = 0; i < playerCameras.Length; i++)
            {
                if (i == index)
                {
                    currentVirtualCamera = playerCameras[i];
                    noise = currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                }
            }
        }
        else if (tag == Tags.enemy)
        {
            for (int i = 0; i < enemyCameras.Length; i++)
            {
                if (i == index)
                {
                    currentVirtualCamera = enemyCameras[i];
                }
            }
        }
        currentVirtualCamera?.gameObject.SetActive(true);
    }

    public void Noise(float amplitudeGain, float frequencyGain, Vector3 pivotOffset = default)
    {
        if (noise == null)
        {
            return;
        }

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
        if (noise == null)
        {
            return;
        }
        noise.enabled = false;
    }
}
