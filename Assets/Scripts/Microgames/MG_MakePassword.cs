using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;


public class MG_MakePassword : Microgame_Base
{
    public string createPasswordText = "Create a Secure Password";
    public string enterPasswordtext = "Enter Your Secure Password";
    public GameObject passwordRequirements;
    public TMP_InputField passwordInput;
    public Button forgotPasswordButton;
    public float createPasswordTimeLimit = 20f;
    public float enterPasswordTimeLimit = 10f;

    PlayerController pc;


    public override bool SetupGame()
    {
        pc = GameManager.Player.GetComponent<PlayerController>();

        microgameTitle = pc.isPasswordSaved ? enterPasswordtext : createPasswordText;
        microgameTimeLimit = pc.isPasswordSaved ? enterPasswordTimeLimit : createPasswordTimeLimit;

        return true;
    }

    public override void StartGame()
    {
        //spawn input field
        passwordRequirements.SetActive(!pc.isPasswordSaved);
        forgotPasswordButton.gameObject.SetActive(pc.isPasswordSaved);

        forgotPasswordButton.onClick.AddListener(OnPasswordReset);

        passwordInput.text = "";
        EventSystem.current.SetSelectedGameObject(passwordInput.gameObject);
        InputManager.GetInputAction(EMinigameAction.SUBMIT).started += OnSubmit;
    }

    bool IsSecurePassword(string password)
	{
        char[] passwordChars = password.ToCharArray();

        List<char> upperChars = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
        List<char> lowerChars = new List<char>("abcdefghijklmnopqrstuvwxyz".ToCharArray());
        List<char> numChars = new List<char>("0123456789".ToCharArray());

        bool hasUpperCase = false;
        bool hasLowerCase = false;
        bool hasNumber = false;
        bool hasSpecialCharacter = false;
        bool atLeast8Chars = password.Length >= 8;

        foreach (char character in passwordChars)
		{
            bool isUpper = upperChars.Contains(character);
            bool isLower = lowerChars.Contains(character);
            bool isNumber = numChars.Contains(character);

            hasUpperCase |= isUpper;
            hasLowerCase |= isLower;
            hasNumber |= isNumber;
            hasSpecialCharacter |= !(isUpper || isLower || isNumber);

        }

        return hasUpperCase && hasLowerCase && hasNumber && hasSpecialCharacter && atLeast8Chars;
	}

    void OnPasswordReset()
	{
        pc.isPasswordSaved = false;
        pc.savedPassword = "";
        microgameWon = false;
        EndGame();
	}

    void OnSubmit(InputAction.CallbackContext context)
    {
        if (pc.isPasswordSaved)
		{
            microgameWon = passwordInput.text == pc.savedPassword;
		}
        else
		{
            microgameWon = IsSecurePassword(passwordInput.text);
		}
        EndGame();
    }

	public override void EndGame()
	{
        if (pc.isPasswordSaved)
		{
            if (microgameWon)
			{
                pc.isPasswordSaved = false;
                pc.savedPassword = "";
			}

            forgotPasswordButton.onClick.RemoveAllListeners();
		}
        else
		{
            if (microgameWon)
			{
                pc.isPasswordSaved = true;
                pc.savedPassword = passwordInput.text;
			}
		}

        InputManager.GetInputAction(EMinigameAction.SUBMIT).started -= OnSubmit;

        base.EndGame();
	}
}
