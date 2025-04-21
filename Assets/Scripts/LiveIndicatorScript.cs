using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(BoxCollider))]
public class LiveIndicatorScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;

    [Header("Timing & Fade")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float coolingDelay = 2f;

    [Header("Intensity Settings")]
    [SerializeField] private float maxWarmIntensity = 1f;
    [SerializeField] private float maxCoolIntensity = 1f;

    [Header("Temperature Values")]
    [SerializeField] private float warmTemperature = 20f;
    [SerializeField] private float coolTemperature = -20f;
    [SerializeField] private float neutralTemperature = 0f;

    [Header("Audio")]
    [SerializeField] private AudioClip coldSound;
    private AudioSource audioSource;

    private bool isPlayerInFireZone = false;
    private Coroutine coolingCoroutine;

    // Post-processing components
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    private WhiteBalance whiteBalance;

    void Start()
    {
        // Try to grab post-processing effects
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out whiteBalance);
        }
        else
        {
            Debug.LogError("Global Volume not assigned to LiveIndicatorScript!");
        }

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ResetPostProcessing();
    }

    private void ResetPostProcessing()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = 0f;
            colorAdjustments.contrast.value = 0f;
        }

        if (whiteBalance != null)
        {
            whiteBalance.temperature.value = neutralTemperature;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0f;
        }
    }

    // Trigger events
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Fire Zone");
            OnPlayerEnterFireZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exited Fire Zone");
            OnPlayerExitFireZone();
        }
    }

    public void OnPlayerEnterFireZone()
    {
        isPlayerInFireZone = true;

        if (coolingCoroutine != null)
        {
            StopCoroutine(coolingCoroutine);
            coolingCoroutine = null;
        }

        StartCoroutine(TransitionToWarmEffect());
    }

    public void OnPlayerExitFireZone()
    {
        isPlayerInFireZone = false;

        StartCoroutine(TransitionToNeutralEffect());

        if (coolingCoroutine != null)
            StopCoroutine(coolingCoroutine);

        coolingCoroutine = StartCoroutine(StartCoolingAfterDelay());
    }

    private IEnumerator StartCoolingAfterDelay()
    {
        yield return new WaitForSeconds(coolingDelay);

        if (!isPlayerInFireZone)
        {
            StartCoroutine(TransitionToCoolEffect());
            
            // Play cold sound when player starts getting cold
            if (coldSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(coldSound);
            }
        }
    }

    private IEnumerator TransitionToWarmEffect()
    {
        float t = 0f;

        float initialSaturation = colorAdjustments?.saturation.value ?? 0f;
        float initialContrast = colorAdjustments?.contrast.value ?? 0f;
        float initialTemperature = whiteBalance?.temperature.value ?? 0f;
        float initialVignette = vignette?.intensity.value ?? 0f;

        while (t < 1f)
        {
            t += fadeSpeed * Time.deltaTime;
            float progress = Mathf.Clamp01(t);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(initialSaturation, 20f * maxWarmIntensity, progress);
                colorAdjustments.contrast.value = Mathf.Lerp(initialContrast, 0.2f * maxWarmIntensity, progress);
            }

            if (whiteBalance != null)
            {
                whiteBalance.temperature.value = Mathf.Lerp(initialTemperature, warmTemperature * maxWarmIntensity, progress);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(initialVignette, 0.2f * maxWarmIntensity, progress);
            }

            yield return null;
        }
    }

    private IEnumerator TransitionToNeutralEffect()
    {
        float t = 0f;

        float initialSaturation = colorAdjustments?.saturation.value ?? 0f;
        float initialContrast = colorAdjustments?.contrast.value ?? 0f;
        float initialTemperature = whiteBalance?.temperature.value ?? 0f;
        float initialVignette = vignette?.intensity.value ?? 0f;

        while (t < 1f)
        {
            t += fadeSpeed * Time.deltaTime;
            float progress = Mathf.Clamp01(t);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(initialSaturation, 0f, progress);
                colorAdjustments.contrast.value = Mathf.Lerp(initialContrast, 0f, progress);
            }

            if (whiteBalance != null)
            {
                whiteBalance.temperature.value = Mathf.Lerp(initialTemperature, neutralTemperature, progress);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(initialVignette, 0f, progress);
            }

            yield return null;
        }
    }

    private IEnumerator TransitionToCoolEffect()
    {
        float t = 0f;

        float initialSaturation = colorAdjustments?.saturation.value ?? 0f;
        float initialContrast = colorAdjustments?.contrast.value ?? 0f;
        float initialTemperature = whiteBalance?.temperature.value ?? 0f;
        float initialVignette = vignette?.intensity.value ?? 0f;

        while (t < 1f)
        {
            t += fadeSpeed * Time.deltaTime;
            float progress = Mathf.Clamp01(t);

            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(initialSaturation, -10f * maxCoolIntensity, progress);
                colorAdjustments.contrast.value = Mathf.Lerp(initialContrast, -0.1f * maxCoolIntensity, progress);
            }

            if (whiteBalance != null)
            {
                whiteBalance.temperature.value = Mathf.Lerp(initialTemperature, coolTemperature * maxCoolIntensity, progress);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(initialVignette, 0.3f * maxCoolIntensity, progress);
            }

            yield return null;
        }
    }

}
