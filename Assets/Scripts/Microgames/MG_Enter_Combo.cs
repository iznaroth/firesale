using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class MG_Enter_Combo : MonoBehaviour
{
    [HideInInspector] public MG_TypeTheWords owner; 
    public TextMeshProUGUI goalTextBox;
    public TextMeshProUGUI playerTextBox;
    public string comboValue;
    private int inputPos = 0;
    private string curText = "";
    public bool isActive = false;

    private void Update()
    {
        playerTextBox.text = curText;
    }
    public void StartGame()
    {
        goalTextBox.text = "<u>" + comboValue + "</u>";
        curText = comboValue;
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
        if (isActive)
        {
            if (ch > 126 || ch < 31)
            {
                return;
            }

            if (comboValue[inputPos] == ch)
            {
                inputPos++;
                curText = curText.Substring(1);
                if (inputPos == comboValue.Length && curText == "")
                {
                    owner.FinishedAMicroGame(true);
                }
            }
        }
    }
}
