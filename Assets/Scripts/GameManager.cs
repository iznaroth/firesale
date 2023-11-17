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
    public GameObject AudioPrefab;

    public delegate void NewAudioSource(AudioClip newClip, float volumeScaler, Vector3 position);
    public static event NewAudioSource newAudioEvent;
    // Start is called before the first frame update
    void Start()
    {
        totalTime = totalGameTime;
        timeRemaining = totalGameTime;
        hpRemaining = 100;
        currentIncome = 666;
        //Player = FindAnyObjectByType<PlayerController>().gameObject;
    }
    private void OnEnable()
    {
        newAudioEvent += SpawnAudioSource;
    }

    private void OnDisable()
    {
        newAudioEvent -= SpawnAudioSource;
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

    public static void SpawnAudio(AudioClip newClip, float volumeScaler, Vector3 position)
    {
        newAudioEvent?.Invoke(newClip,volumeScaler, position);
    }

    public void SpawnAudioSource(AudioClip newClip, float volumeScaler, Vector3 position)
    {
       GameObject newAudioObject = Instantiate(AudioPrefab);
       newAudioObject.GetComponent<SoundScript>().PlayAudio(newClip, volumeScaler);
    }
}
