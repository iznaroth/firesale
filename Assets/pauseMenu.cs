using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{

    public Slider volSlider;

    void Awake(){

        GameManager.instance.slider = volSlider;
        Debug.Log("Paused!");
    }
}
