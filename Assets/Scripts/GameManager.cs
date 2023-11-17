using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameObject Player;
    public static float timeRemaining;
    public static float curiosRemaining;
    public static int currentCuriosValue;
    public static int hpRemaining;
    public static int currentIncome = 666;
    public static bool pauseTimer = false;

    public static float totalTime = 900;
    public float totalGameTime = 900;
    // Start is called before the first frame update
    void Start()
    {
        totalTime = totalGameTime;
        timeRemaining = totalGameTime;
        hpRemaining = 100;
        currentIncome = 666;
        //Player = FindAnyObjectByType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseTimer)
        {
            timeRemaining -= Time.deltaTime;
        }    
        if(timeRemaining < 0)
        {
            EndGame();
        }
    }


    public void EndGame()
    {
        Debug.Log("GAME OVER");
        //go to stats screen
    }

    public void StartGame()
    {
        timeRemaining = totalGameTime;
        hpRemaining = 100;
        currentIncome = 666;
    }

    public static void IncreaseMoney()
    {
       currentIncome += currentCuriosValue;
    }
    public static void DecreaseMoney(int itemCost)
    {
        currentIncome -= itemCost;
    }
}
