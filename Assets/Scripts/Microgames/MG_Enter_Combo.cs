using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MG_Enter_Combo : Microgame_Base
{
    public TextMeshProUGUI goalTextBox;
    public TextMeshProUGUI playerTextBox;
    public string comboValue;
    public bool backspaceAllowed = false;
    public bool wrongKeyStillIncrements = true;
    private int inputPos = 0;
    private string curText = "";


    private void Update()
    {
        //do we give a shit about repeat inputs?
        //add a thing for underlining
        if (backspaceAllowed && Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            Debug.Log("FUCK!");
            curText = curText.Substring(0, curText.Length - 2);
        }
        playerTextBox.text = curText;
    }
    public override void StartGame()
    {
        Debug.Log("add a thing for underlining the wrong characters 4 eyes");
        goalTextBox.text = comboValue;
    }
    public override bool EndGame()
    {
        Debug.Log("Result: " + microgameWon);
        Keyboard.current.onTextInput -= OnTextInput;
        Destroy(this.gameObject);
        return microgameWon;
    }


    protected void OnEnable()
    {
        Keyboard.current.onTextInput += OnTextInput;
    }

    protected void OnDisable()
    {
        Keyboard.current.onTextInput -= OnTextInput;
    }

    public void OnTextInput(char ch)
    {
        if(ch == null)
        {
            return;
        }

        //Player has typed past the character count but can backspace so fuck it they just keep typing
        if(inputPos >= comboValue.Length && backspaceAllowed)
        {
            ++inputPos;
            curText += ch;
            microgameWon = false;
            return;
        }

        if (comboValue[inputPos] == ch)
        {
            ++inputPos;
            curText += ch;
            if (inputPos == comboValue.Length)
            {
                microgameWon = true;
                EndGame();
            }  
        }
        else
        {
            microgameWon = false;
            //put that character in, but don't move forward forcing the player to type that shit right if they can backspace
            if (backspaceAllowed)
            {
                curText += ch;
                if (wrongKeyStillIncrements)
                {
                    inputPos++;
                }
            }
            else
            {
                EndGame();
            } 
        }
    }
}
