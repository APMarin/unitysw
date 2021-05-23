using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixer sfxMixer;
    public void setVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void setSfx(float volume)
    {
        sfxMixer.SetFloat("bgmvolume", volume);
    }

}
