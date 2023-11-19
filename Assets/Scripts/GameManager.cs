using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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

    public static bool didWin;
    public static float finalTime;
    public static float finalBalance;
    public static float finalCurios;

    public delegate void NewAudioSource(AudioClip newClip, float volumeScaler, float newPitch, Vector3 position);
    public static event NewAudioSource newAudioEvent;


	private void Awake()
	{
        instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        totalTime = totalGameTime;
        timeRemaining = totalGameTime;
        hpRemaining = 100;
        currentIncome = 666;
        curiosRemaining = 20;
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
            EndGame(false);
        }
    }


    public void EndGame(bool winOrLose)
    {
        Debug.Log("GAME OVER");
        //go to stats screen
        finalTime = timeRemaining;
        finalBalance = currentIncome;
        finalCurios = curiosRemaining;
        didWin = winOrLose;
        SceneManager.LoadSceneAsync("EndGame");
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

    public static void SpawnAudio(AudioClip newClip, float volumeScaler, float pitch, Vector3 position)
    {
        newAudioEvent?.Invoke(newClip, volumeScaler, pitch, position);
    }

    public void SpawnAudioSource(AudioClip newClip, float volumeScaler, float newPitch, Vector3 position)
    {
       GameObject newAudioObject = Instantiate(AudioPrefab, position, Quaternion.identity);
       newAudioObject.GetComponent<SoundScript>().PlayAudio(newClip, volumeScaler, newPitch);
    }
}
