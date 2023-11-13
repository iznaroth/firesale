using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicrogameManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Slider timerBar;
    public Microgame_Base currentGame;

    public float timeLimit;
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
        timerBar.value = timeLeft / timeLimit;
        if(timeLeft < 0)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        titleText.text = currentGame.microgameTitle;
        timeLimit = currentGame.microgameTimeLimit;
        timeLeft = timeLimit;
        currentGame.StartGame();
    }

    public bool EndGame()
    {
        currentGame.EndGame();
        return false;
    }
}
