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

    //private void Start()
    //{
    //    Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
            if (hidesShop){ StartCoroutine(HideShop()); }
            else { StartCoroutine(ShowShop()); }
            PlayerController.inShop = isShop;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
            if (hidesShop) { StartCoroutine(HideShop()); }
            else { StartCoroutine(ShowShop()); }
            PlayerController.inShop = isShop;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(false);
            if (hidesShop) { StartCoroutine(HideShop()); }
            else { StartCoroutine(ShowShop()); }
            if (isShop)
            {
                PlayerController.inShop = false;
            }
        }
    }

    private IEnumerator HideShop(){
        while(sRend.color.a < 1f){
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a + 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ShowShop(){
        while(sRend.color.a > 0f){
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a - 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
