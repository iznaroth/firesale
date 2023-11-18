using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpAndOut : MonoBehaviour
{
    public TMP_Text text;

    void Awake()
    {
        StartCoroutine(fadeOut());
    }

    public IEnumerator fadeOut(){
        while(this.text.alpha > 0){
            this.text.alpha -= 0.05f;
            this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y + 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
    }

}
