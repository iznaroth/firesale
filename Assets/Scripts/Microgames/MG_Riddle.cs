using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Riddle
{
    public string riddleText;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
    [Range(1,4)]
    public int rightAnswer;
    public float timeLimit;
}

public class MG_Riddle : Microgame_Base
{
    public List<Riddle> riddles;
    public TextMeshProUGUI riddleText;
    public TextMeshProUGUI option1Text;
    public Button option1Button;
    public TextMeshProUGUI option2Text;
    public Button option2Button;
    public TextMeshProUGUI option3Text;
    public Button option3Button;
    public TextMeshProUGUI option4Text;
    public Button option4Button;

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
        microgameTimeLimit = chosenRiddle.timeLimit;

        return true;
    }

    public void ChoseIncorrect()
	{
        microgameWon = false;
        EndGame();
	}

    public void ChoseCorrect()
	{
        microgameWon = true;
        availableRiddles.Remove(chosenRiddle);
        EndGame();
	}


    public override void StartGame()
    {
        //spawn input field
        riddleText.text = chosenRiddle.riddleText;
        option1Text.text = chosenRiddle.option1;
        option2Text.text = chosenRiddle.option2;
        option3Text.text = chosenRiddle.option3;
        option4Text.text = chosenRiddle.option4;

        option1Button.onClick.AddListener(chosenRiddle.rightAnswer == 1 ? ChoseCorrect : ChoseIncorrect);
        option2Button.onClick.AddListener(chosenRiddle.rightAnswer == 2 ? ChoseCorrect : ChoseIncorrect);
        option3Button.onClick.AddListener(chosenRiddle.rightAnswer == 3 ? ChoseCorrect : ChoseIncorrect);
        option4Button.onClick.AddListener(chosenRiddle.rightAnswer == 4 ? ChoseCorrect : ChoseIncorrect);

        TextMeshProUGUI[] texts = { riddleText, option1Text, option2Text, option3Text, option4Text };
        foreach (TextMeshProUGUI text in texts)
		{
            TypewriterEffect typewriter = text.GetComponent<TypewriterEffect>();
            if (typewriter != null)
            {
                typewriter.NewText(text.text);
            }
        }
    }

    private void Update()
    {
        //check if player hits enter or clicks submit or something
    }

    public virtual void EndGame()
    {
        base.EndGame();

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();
        option4Button.onClick.RemoveAllListeners();
    }
}
