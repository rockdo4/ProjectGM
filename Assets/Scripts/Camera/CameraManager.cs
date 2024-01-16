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

    private CinemachineVirtualCamera currentCamera;
    private int previousPriority;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        SetCameraWithTag(Tags.player);
    }

    public void SetCameraWithTag(string tag, int index = 0)
    {
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }
        currentCamera = GameObject.FindWithTag(tag)?.GetComponentsInChildren<CinemachineVirtualCamera>(true)?[index];
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(true);
        }
    }

    public void ChangeCamera(string tag, int index = 0)
    {
        if (currentCamera != null)
        {
            currentCamera.Priority = previousPriority;
        }
        currentCamera = GameObject.FindWithTag(tag)?.GetComponentsInChildren<CinemachineVirtualCamera>()?[index];
        if (currentCamera != null)
        {
            previousPriority = currentCamera.Priority;
        }
    }
    public void SetPriority(int priority)
    {
        if (currentCamera == null)
        {
            return;
        }
        previousPriority = priority;
        currentCamera.Priority = priority;
    }

    public void Noise(float amplitudeGain, float frequencyGain, Vector3 pivotOffset = default)
    {
        var noise = currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
    }

    public void StopNoise()
    {
        var noise = currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            return;
        }
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
        noise.m_PivotOffset = Vector3.zero;
    }

    public void OnNoise(bool active)
    {
        var noise = currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            return;
        }
        noise.enabled = active;
    }
}
