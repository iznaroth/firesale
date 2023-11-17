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

    //private void Start()
    //{
    //    Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
            if (hidesShop){ HideShop(); }
            PlayerController.inShop = isShop;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
            if (hidesShop) { HideShop(); }
            PlayerController.inShop = isShop;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(false);
            if (hidesShop) { HideShop(); }
            if (isShop)
            {
                PlayerController.inShop = false;
            }
        }
    }

    private void HideShop()
    {
        // hide shop anim goes here
    }
}
