using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Microgame_Base : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Slider timerBar;

    public string microgameTitle;
    public float microgameTimeLimit;
    public int microgameDifficulty; // difficulty of completion, used to determine if player has to do another microgame to convince the npc
    private bool microgameWon = false;


    private float timeLeft = 10;

    private void Start()
    {
        if(titleText.gameObject == null || timerBar.gameObject == null)
        {
            Debug.Log("SHID! SOMEONE FORGOT TO SET THE OBJECTS ON: " + this.gameObject.name + ". \n WHAT A MORON!");
            titleText = this.GetComponentInChildren<TextMeshProUGUI>();
            timerBar = this.GetComponentInChildren<Slider>();
        }
        StartGame();
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        timerBar.value = timeLeft / microgameTimeLimit;
        if(timeLeft < 0)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        timeLeft = microgameTimeLimit;
    }

    public bool EndGame()
    {
        return false;
    }
}
