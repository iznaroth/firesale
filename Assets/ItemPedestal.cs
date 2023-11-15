using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject storedItem;
    public GameObject interactIcon;
    bool actionable = false;
    private PlayerController pl;
    void Awake(){
        PlayerController.interactEvent += PickUp;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.name == "Player"){
            pl = col.gameObject.GetComponent<PlayerController>();
            interactIcon.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.name == "Player"){
            pl = null;
            interactIcon.SetActive(false);
        }
    }
    private void PickUp(GameObject toSwap){
        if(pl != null){
            Vector3 hold = this.storedItem.transform.position;
            this.storedItem.transform.position = toSwap.transform.position;
            toSwap.transform.position = hold;

            toSwap.transform.SetParent(this.gameObject.transform);
            this.storedItem.transform.SetParent(pl.gameObject.transform);

            pl.setHolding(this.storedItem);
            this.storedItem = toSwap;

            //disable animation clip
        }
    }

    public void OnDisable(){
        PlayerController.interactEvent -= PickUp;
    }

}
