using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject faceImage;
    public GameObject handImage;
    public Button startButton;

    public float handSpeed = 1;

    public bool startupDone = false;
    public string nextScene = "";
    public bool playingStartup;

    // Start is called before the first frame update
    void Awake()
    {
        startupDone = false;
        startButton.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playingStartup) { SetHandPosition(); }
        if (startupDone)
        {
            startupDone = false;
            SwitchScenes();
        }
    }

    public void PlayAnimation()
    {
        this.GetComponentInChildren<Animation>().Play();
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
