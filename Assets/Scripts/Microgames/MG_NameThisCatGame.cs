using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MG_NameThisCatGame : Microgame_Base
{
    public Sprite[] catPics;
    public Image catImage;
    public TMP_InputField nameInput;

    List<Sprite> availableCats = new List<Sprite>();

    public override bool SetupGame()
    {
        if (availableCats.Count < 1)
		{
            availableCats.AddRange(catPics);
		}
        // draw a cat pick from a array and set it to the cat we want
        if (availableCats.Count < 1) return false;

        int catIndex = Random.Range(0, availableCats.Count);
        catImage.sprite = availableCats[catIndex];
        availableCats.RemoveAt(catIndex);

        return true;
    }

    public override void StartGame()
    {
        //spawn input field
        nameInput.text = "";
        EventSystem.current.SetSelectedGameObject(nameInput.gameObject);
        InputManager.GetInputAction(EMinigameAction.SUBMIT).started += OnSubmit;
    }

    void OnSubmit(InputAction.CallbackContext context)
	{
        microgameWon = nameInput.text.Length > 0;
        EndGame();
	}
}
