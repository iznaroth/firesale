using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoomChange : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();
    //thanks BenBonk
    public GameObject virtualCam;
    public bool hidesShop = false;
    public bool isShop = false;
    public SpriteRenderer sRend;
    public static bool startingFadeTimer;
    
    private bool isCoroutineRunning = false;

    private void Start()
    {
        //    Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
        startingFadeTimer = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (hidesShop) { StartCoroutine(HideShop()); }
            else { StartCoroutine(ShowShop()); }
            virtualCam.SetActive(true);
            PlayerController.inShop = isShop;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            //if (hidesShop) { StartCoroutine(HideShop()); }
            //else { StartCoroutine(ShowShop()); }
            virtualCam.SetActive(true);
            PlayerController.inShop = isShop;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            //if (hidesShop) { StartCoroutine(HideShop()); }
            //else { StartCoroutine(ShowShop()); }
            virtualCam.SetActive(false);
            if (isShop)
            {
                PlayerController.inShop = false;
            }
        }
    }

    private IEnumerator HideShop(){
        Debug.Log("Hiding!");

        if(isCoroutineRunning){
            yield break;
        }

        isCoroutineRunning = true;
        while(sRend.color.a < 1f){
            Debug.Log("inc alpha!");
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a + 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        isCoroutineRunning = false;
    }

    private IEnumerator ShowShop(){

        Debug.Log("Showing!");
        
        if(isCoroutineRunning){
            yield break;
        }

        isCoroutineRunning = true;
        if (startingFadeTimer)
        {
            while (sRend.color.a > 0f)
            {
                Debug.Log("inc alpha!");
                sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a - 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (sRend.color.a > 0f)
            {
                Debug.Log("dec alpha!");
                sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a - 0.05f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        startingFadeTimer = false;
        isCoroutineRunning = false;
    }
}
