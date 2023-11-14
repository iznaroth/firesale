using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject storedItemSprite;
    public GameObject interactIcon;
    public Item itemStored;

    bool actionable = false;

    private PlayerController pl;

    void Awake(){
        PlayerController.interactEvent += PickUp;
    }


    // Update is called once per frame
    void Update()
    {
        //Item float anim based on properties


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

    public void setItem(Item to){
        this.itemStored = to;
        this.storedItemSprite.GetComponent<SpriteRenderer>().sprite = (itemStored == null) ? null : itemStored.sprite;
    }
    
    private void PickUp(){
        Debug.Log("Pickup fired.");
        if(pl != null){
            this.setItem(pl.getHolding());
            pl.setHolding(this.itemStored);
        }
    }

    public void OnDisable(){
        PlayerController.interactEvent -= PickUp;
    }

}
