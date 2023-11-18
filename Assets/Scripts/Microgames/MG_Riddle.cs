using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public struct Riddle
{
    public string riddleText;
    public string[] riddleAnswers;
    public int rightAnswer;
}

public class MG_Riddle : Microgame_Base
{
    public List<Riddle> riddles;
    public TextMeshProUGUI riddleText;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;
    public TextMeshProUGUI option4Text;

    static List<Riddle> availableRiddles = new List<Riddle>();

    Riddle chosenRiddle;

    public override bool SetupGame()
    {
        // draw a cat pick from a array and set it to the cat we want
        if (availableRiddles.Count < 1)
		{
            availableRiddles.AddRange(riddles);
		}

        if (availableRiddles.Count < 1)
		{
            Debug.LogError("No riddles available!");
            return false;
		}

        chosenRiddle = availableRiddles[Random.Range(0, availableRiddles.Count)];
        availableRiddles.Remove(chosenRiddle);

        return true;
    }

    public void ChoseIncorrect()
	{

	}

    public void ChoseCorrect()
	{

	}


    public override void StartGame()
    {
        //spawn input field
    }

    private void Update()
    {
        //check if player hits enter or clicks submit or something
    }

    public virtual void EndGame()
    {
        base.EndGame();
    }
}
