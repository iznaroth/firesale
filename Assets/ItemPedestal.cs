using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject storedItem;
    //public GameObject interactIcon;
    bool actionable = false;
    private PlayerController pl;

    public float startDelay;
    public bool isClosest = false;
    
    void Awake(){
        PlayerController.interactEvent += PickUp;
        StartCoroutine(animOffset());
    }

   void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.tag == "Player"){ //using name here is bad, use tags
            pl = col.gameObject.GetComponent<PlayerController>();
            //interactIcon.SetActive(true);
        }
    }

/*    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.name == "Player"){
            pl = null; //?!??!??? what is happening here why would we do this???????
            //interactIcon.SetActive(false);
        }
    }*/
    public void PickUp(GameObject toSwap){
        if (pl != null && isClosest){
            Vector3 hold = this.storedItem.transform.position;
            this.storedItem.transform.position = toSwap.transform.position;
            toSwap.transform.position = hold;

            toSwap.transform.SetParent(this.gameObject.transform);
            this.storedItem.transform.SetParent(pl.gameObject.transform);

            pl.setHolding(this.storedItem);
            this.storedItem = toSwap;
            this.storedItem.transform.eulerAngles = new Vector3(0, 0, this.storedItem.GetComponent<Item>().spritePedestalRotation);
            isClosest = false;
            //disable animation clip
        }
    }

    public void OnDisable(){
        PlayerController.interactEvent -= PickUp;
    }

    public IEnumerator animOffset(){
        yield return new WaitForSeconds(startDelay);
        this.GetComponent<Animation>().Play();
    }

}
