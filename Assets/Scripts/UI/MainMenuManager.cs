using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject faceImage;
    public GameObject handImage;
    public Button startButton;

    public float handSpeed = 1;

    public bool startupDone = false;
    public string nextScene = "";
    public bool playingStartup;

    public bool isEndscreen;

    public GameObject victory;
    public GameObject defeat;

    public TextMeshProUGUI timeLeft;
    public TextMeshProUGUI money;
    public TextMeshProUGUI relicsLeft;
    public TextMeshProUGUI victoryText;

    private bool loadingNewScene = false;

    // Start is called before the first frame update
    void Awake()
    {
        startupDone = false;
        startButton.enabled = true;

        if(isEndscreen){
            victory.SetActive(GameManager.didWin);
            defeat.SetActive(!GameManager.didWin);
            if (GameManager.didWin)
            {
                victoryText.gameObject.GetComponent<TypewriterEffect>().NewText(victoryText.text);
            }
            else
            {
                relicsLeft.gameObject.GetComponent<TypewriterEffect>().NewText(relicsLeft.text.Replace("{curios}", GameManager.finalCurios.ToString()));
            }
            timeLeft.text = "" +  GameManager.finalTime;
            money.text =  "" + GameManager.finalBalance;
        }
        loadingNewScene = false;
        SceneManager.sceneLoaded += SceneLoaded;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEndscreen){
            if (!playingStartup) { SetHandPosition(); }
            if (startupDone && !loadingNewScene)
            {
                Debug.Log("Queue Sceneload Async");
                loadingNewScene = true;
                startupDone = false;
                SwitchScenes();
            }
        }
    }

    public void PlayAnimation()
    {
        if(!isEndscreen){
            this.GetComponentInChildren<Animation>().Play();
        }
    }

    public void SetHandPosition()
    {
        Vector3 goalVector = (Input.mousePosition + faceImage.transform.position) / 2;
        handImage.transform.position = Vector3.Lerp(handImage.transform.position, goalVector, handSpeed * Time.deltaTime);
        handImage.transform.right = Input.mousePosition - faceImage.transform.position;
    }

    public void SwitchScenes()
    {
        SceneManager.LoadSceneAsync(nextScene);
    }

    public void SwitchScenesDirect(string name)
    {

        SceneManager.LoadSceneAsync(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SceneLoaded(Scene scene, LoadSceneMode mode) { loadingNewScene = false; }
}
