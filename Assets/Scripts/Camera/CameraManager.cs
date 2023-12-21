using Cinemachine;
using System.Collections;
using Unity.Mathematics;
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

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
