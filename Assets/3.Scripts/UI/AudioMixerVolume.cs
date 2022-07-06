using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerVolume : MonoBehaviour
{
    public Slider slider;
    public AudioMixer mixer;
    public string audioMixerName;

    private void Start()
    {
        float sliderValue = PlayerPrefs.GetFloat($"audioMixerVolume-{audioMixerName}", 1);
        if (slider)
            slider.value = sliderValue;

        SetLevel(sliderValue);
    }

    public void SetLevel(float sliderVal)
    {
        mixer.SetFloat(audioMixerName, Mathf.Log10(sliderVal) * 20);
        PlayerPrefs.SetFloat($"audioMixerVolume-{audioMixerName}", sliderVal);
    }
}
