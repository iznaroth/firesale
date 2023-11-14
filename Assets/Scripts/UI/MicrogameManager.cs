using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicrogameManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statusText;
    public Slider timerBar;
    public TextMeshProUGUI timerText;
    public Microgame_Base currentGame;
    public Microgame_Base[] AllMicrogames; // TODO UPDATE THIS TO AUTO ADD
    public Dictionary<NPC_Types, List<Microgame_Base>> gamesDict = new Dictionary<NPC_Types, List<Microgame_Base>>();
    private Animator anime;

    private float timeLimit;
    public static bool pauseTimer = false;

    private float timeLeft = 10f;

    //Randomizing, sorting, etc.
    private static System.Random rng = new System.Random();
    private List<Microgame_Base> currentList = new List<Microgame_Base>();

    public string[] winPhrases;
    public string[] losePhrases;

    private void Start()
    {
        if (titleText.gameObject == null || timerBar.gameObject == null || statusText.gameObject == null)
        {
            Debug.Log("SHID! SOMEONE FORGOT TO SET THE OBJECTS ON: " + this.gameObject.name + ". \n WHAT A MORON!");
        }

        anime = this.GetComponent<Animator>();
        //might need to generate initial queues   
        foreach (Microgame_Base mg in AllMicrogames)
        {
            foreach (NPC_Types types in GetFlags(mg.npcType))
            {
                gamesDict.TryGetValue(types, out currentList);
                if (currentList == null)
                {
                    currentList = new List<Microgame_Base>();
                }
                currentList.Add(mg);
                IListExtensions.Shuffle(currentList);
                gamesDict.Remove(types);
                gamesDict.TryAdd(types, currentList);
            }
        }
        statusText.gameObject.SetActive(true);
        this.transform.GetChild(0).transform.localScale = Vector3.zero;
    }
    private IEnumerable<NPC_Types> GetFlags(NPC_Types input)
    {
        foreach (NPC_Types value in NPC_Types.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }

    private void Update()
    {
        if (!pauseTimer)
        {
            timeLeft -= Time.deltaTime;
            timerBar.value = timeLeft / timeLimit;
            float roundedTime = Mathf.Round(timeLeft);
            if (roundedTime <= 5)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = roundedTime.ToString();
                if (roundedTime <= 3)
                {
                    timerText.text += "!";
                }
            }
            if (timeLeft < 0)
            {
                EndGame();
            }
        }
        if (!DialogueManager.newDialogueStarted && DialogueManager.dialogueState >= 3)
        {
            DialogueManager.dialogueState = 0;
            anime.Play("MG_Out");
        }
    }

    public void StartNewGame()
    {
        anime.Play("MG_In");
        statusText.text = currentGame.microgameTitle;
        Debug.Log(statusText.text);
        StartCoroutine("StartDelay");
    }

    public void GameResult(bool result)
    {
        pauseTimer = true;
        currentGame.EndGame();
        titleText.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);

        if (result)
        {

            statusText.GetComponent<TypewriterEffect>().NewText(winPhrases[Random.Range(0, winPhrases.Length - 1)]);
        }
        else
        {

            statusText.GetComponent<TypewriterEffect>().NewText(losePhrases[Random.Range(0, winPhrases.Length - 1)]);

        }

        DialogueManager.wonLastMicrogame = result;
        DialogueManager.dialogueState++;
    }

    public void EndGame()
    {

    }


    public Microgame_Base GetNewMicrogame(NPC_Types npcType)
    {
        currentList = null;
        Microgame_Base newGame;
        gamesDict.TryGetValue(npcType, out currentList);
        newGame = Instantiate(currentList[0]);
        currentList.RemoveAt(0);
        currentList.Add(newGame);
        gamesDict.Remove(npcType);
        gamesDict.TryAdd(npcType, currentList);
        statusText.text = newGame.microgameTitle;
        statusText.gameObject.SetActive(true);
        currentGame = newGame;
        return newGame;
    }

    IEnumerator StartDelay()
    {
        pauseTimer = true;
        timerBar.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        statusText.text = currentGame.microgameTitle;
        bool timeWaster = currentGame.SetupGame();
        yield return new WaitForSeconds(currentGame.microgameStartDelay);
        currentGame.gameObject.SetActive(true);
        statusText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(true);
        titleText.text = currentGame.microgameTitle;
        timeLimit = currentGame.microgameTimeLimit;
        timeLeft = timeLimit;
        timerBar.gameObject.SetActive(true);
        currentGame.StartGame();
        pauseTimer = false;
    }
}
public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}