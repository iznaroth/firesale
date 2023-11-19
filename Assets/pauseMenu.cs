using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{

    public Slider musicVolSlider;
    public Slider sfxVolSlider;

    void Awake(){
        
        //Debug.Log("PAUSE AWAKE");
        float volSetting;
        GameManager.instance.masterMixer.GetFloat("musicVol", out volSetting);
        musicVolSlider.value = Mathf.Exp( volSetting / 20);
        //Debug.Log(volSlider.value);
        GameManager.instance.musicSoundSlider = musicVolSlider;
        GameManager.instance.masterMixer.GetFloat("sfxVol", out volSetting);
        sfxVolSlider.value = Mathf.Exp(volSetting / 20);
        //Debug.Log(volSlider.value);
        GameManager.instance.sfxSoundSlider = sfxVolSlider;
        //Debug.Log("Paused!");
    }

    void OnEnable(){
        PauseGame();
    }

    void OnDisable(){
        ResumeGame();
    }

    void PauseGame ()
    {
        Time.timeScale = 0;
    }

    void ResumeGame ()
    {
        Time.timeScale = 1;
    }
}
