using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private float defaultIntensity = 3f;
    [SerializeField] private float defaultDuration = 0.2f;

    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private void Awake()
    {
        instance = this;

        noise = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0;
    }

    public void Shake(float intensity = -1, float duration = -1)
    {
        float shakeIntensity = (intensity > 0) ? intensity : defaultIntensity;
        float shakeDuration = (duration > 0) ? duration : defaultDuration;

        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(shakeIntensity, shakeDuration));
    }
    public IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        noise.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        noise.m_AmplitudeGain = 0; // Reseta o shake
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Shake();
        }
    }
}
