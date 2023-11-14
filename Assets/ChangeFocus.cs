using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeFocus : MonoBehaviour
{

    public CinemachineVirtualCamera cam;
    public Transform target;

    public bool inOrOut;

    public SpriteRenderer sRend;

    public GameObject mutual;

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D col)
    {
        cam.m_LookAt = target;
        cam.m_Follow = target;

        if(inOrOut){
            StartCoroutine(fadeIn());
        } else {
            StartCoroutine(fadeOut());
        }
    }

    private IEnumerator fadeIn(){
        while(sRend.color.a < 1f){
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a + 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        endCoroutine();
    }

    private IEnumerator fadeOut(){
        while(sRend.color.a > 0f){
            sRend.color = new Color(sRend.color.r, sRend.color.g, sRend.color.b, sRend.color.a - 0.05f);
            yield return new WaitForSeconds(0.01f);
        }

        endCoroutine();
    }

    private void endCoroutine(){
        this.gameObject.SetActive(false);
        mutual.SetActive(true);
    }
}
