using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MG_TypeTheWords : Microgame_Base
{
    public int wordAmount = 3;
    public Vector2[] popupPositions;
    public float positionRandomizationAmountX = 400;
    public float positionRandomizationAmountY = 50;
    public GameObject wordMicrogameObject;
    public string[] wordList;
    private Queue<string> selectedWords = new Queue<string>();
    private Stack<int> selectedPositions = new Stack<int>();
    private GameObject[] wordsToType;
    private int currentWordIndex = 0;
    private MG_Enter_Combo currentCombo;

    private void Update()
    {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            EndGame();
        }
    }
    public override void StartGame()
    {
        wordsToType = new GameObject[wordAmount];
        string tempWord = wordList[Random.Range(0, wordList.Length - 1)];
        int tempInt = Random.Range(0, popupPositions.Length - 1);
        selectedWords.Enqueue(tempWord);
        selectedPositions.Push(tempInt);
        for (int i = 1; i < wordAmount; i++)
        {
            tempWord = wordList[Random.Range(0, wordList.Length - 1)];
            tempInt = Random.Range(0, popupPositions.Length - 1);
            while (selectedWords.Contains(tempWord))
            {
                tempWord = wordList[Random.Range(0, wordList.Length - 1)];
            }
            while (selectedPositions.Contains(tempInt))
            {
                tempInt = Random.Range(0, popupPositions.Length - 1);
            }
            selectedWords.Enqueue(tempWord);
            selectedPositions.Push(tempInt);
        }
        for (int i = 0; i < wordAmount; i++)
        {
            wordsToType[i] = Instantiate(wordMicrogameObject, this.transform.GetChild(0).transform, true);
            wordsToType[i].GetComponent<RectTransform>().localPosition = Vector3.zero;
            wordsToType[i].transform.localScale = Vector3.one;
            wordsToType[i].transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one;
            wordsToType[i].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one;
            wordsToType[i].GetComponent<MG_Enter_Combo>().comboValue = selectedWords.Dequeue();
            wordsToType[i].GetComponent<MG_Enter_Combo>().StartGame();
            wordsToType[i].GetComponent<MG_Enter_Combo>().owner = this;

            if (i == wordAmount - 1)
            {
                wordsToType[i].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localPosition = Vector2.zero;
            }
            else
            {
                wordsToType[i].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localPosition = popupPositions[selectedPositions.Pop()];
            }
        }
        currentCombo = wordsToType[wordAmount - 1].GetComponent<MG_Enter_Combo>();
        currentCombo.isActive = true;
        currentCombo.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
        currentWordIndex = wordAmount - 1;
        microgameWon = false;
    }

    public void FinishedAMicroGame(bool result)
    {
        if (result)
        {
            if (currentWordIndex <= 0)
            {
                wordsToType[currentWordIndex].SetActive(false);
                microgameWon = true;
                Debug.Log("Huh?");
                EndGame();
            }
            else
            {
                Debug.Log("Huh?23");
                wordsToType[currentWordIndex].SetActive(false);
                currentWordIndex--;

                currentCombo = wordsToType[currentWordIndex].GetComponent<MG_Enter_Combo>();
                currentCombo.isActive = true;
                currentCombo.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one * 1.5f;
            }
        }
        else
        {
            microgameWon = false;
            EndGame();
        }
    }
}
