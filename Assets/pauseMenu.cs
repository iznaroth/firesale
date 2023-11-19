using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{

    public Slider volSlider;

    void Awake(){

        Debug.Log("PAUSE AWAKE");
        float volSetting;
        GameManager.instance.masterMixer.GetFloat("musicVol", out volSetting);
        volSlider.value = Mathf.Exp( volSetting / 20);
        Debug.Log(volSlider.value);
        GameManager.instance.slider = volSlider;
        Debug.Log("Paused!");
    }
}
