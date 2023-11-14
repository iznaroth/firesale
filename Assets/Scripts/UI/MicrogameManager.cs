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
    public Microgame_Base[] AllMicrogames; // TODO UPDATE THIS TO AUTO ADD
    public Dictionary<NPC_Types, List<Microgame_Base>> gamesDict = new Dictionary<NPC_Types, List<Microgame_Base>>();

    private float timeLimit;
    public static bool pauseTimer = false;

    private float timeLeft = 10f;

    //Randomizing, sorting, etc.
    private static System.Random rng = new System.Random();
    private List<Microgame_Base> currentList = new List<Microgame_Base>();

    private void Start()
    {
        if(titleText.gameObject == null || timerBar.gameObject == null)
        {
            Debug.Log("SHID! SOMEONE FORGOT TO SET THE OBJECTS ON: " + this.gameObject.name + ". \n WHAT A MORON!");
            titleText = this.GetComponentInChildren<TextMeshProUGUI>();
            timerBar = this.GetComponentInChildren<Slider>();
        }



        //might need to generate initial queues   
        foreach(Microgame_Base mg in AllMicrogames)
        {
            foreach(NPC_Types types in GetFlags(mg.npcType))
            {
                gamesDict.TryGetValue(types, out currentList);
                if(currentList == null)
                {
                    currentList = new List<Microgame_Base>();
                }
                currentList.Add(mg);
                IListExtensions.Shuffle(currentList);
                gamesDict.Remove(types);
                gamesDict.TryAdd(types, currentList);
            }
        }
        this.gameObject.SetActive(false);
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
            if(timeLeft < 0)
            {
                EndGame();
            }
        }
    }

    public void StartNewGame()
    {
        titleText.text = currentGame.microgameTitle;
        timeLimit = currentGame.microgameTimeLimit;
        timeLeft = timeLimit;
        currentGame.StartGame();
        pauseTimer = false;
    }

    public void GameResult(bool result)
    {
        if (result)
        {
            titleText.text = "Success!";
            pauseTimer = true;
            currentGame.EndGame();
        }
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
        return newGame;
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