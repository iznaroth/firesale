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
    private int inputPos = 0;
    private string curText = "";


    private void Update()
    {
        //do we give a shit about repeat inputs?
        //add a thing for underlining
        //what happens with characters not in the text asset?
        playerTextBox.text = curText;
    }
    public override void StartGame()
    {
        Debug.Log("add a thing for underlining the wrong characters 4 eyes, and also characters not in the unicode asset for text");
        goalTextBox.text = "<u>" + comboValue + "</u>";
        curText = comboValue;
        microgameWon = false;
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
        Debug.Log(inputPos);
        if(ch > 126 || ch < 33) 
        { 
            return;
        }

        if (comboValue[inputPos] == ch)
        {
            inputPos++;
            curText = curText.Substring(1);
            if (inputPos == comboValue.Length && curText == "")
            {
                microgameWon = true;
                EndGame();
            }  
        }
    }
}
