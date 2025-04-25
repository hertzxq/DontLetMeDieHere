using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    private const string volumeParam = "MusicVolume";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(volumeParam, 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(volumeParam, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(volumeParam, volume);
        PlayerPrefs.Save();
    }
}
