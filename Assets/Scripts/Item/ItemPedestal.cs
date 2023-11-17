using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemPedestal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject storedItem;
    //public GameObject interactIcon;
    bool actionable = false;
    private PlayerController pl;
    private GameObject nameplate;

    public float startDelay;
    public bool isClosest = false;
    private bool playerInRange = false;
    
    void Awake(){
        PlayerController.interactEvent += PickUp;
        StartCoroutine(animOffset());
        nameplate = GetComponentInChildren<Canvas>().gameObject;
        nameplate.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + storedItem.GetComponent<Item>().name + "-\n-" + storedItem.GetComponent<Item>().value + "-";
    }

    private void Update()
    {
        if(isClosest && playerInRange && storedItem != null)
        {
            nameplate.SetActive(true);
        }
        else
        {

            nameplate.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.tag == "Player"){ //using name here is bad, use tags
            pl = col.gameObject.GetComponent<PlayerController>();
            nameplate.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + storedItem.GetComponent<Item>().name + "-\n-" + storedItem.GetComponent<Item>().value + "-";
            playerInRange = true;
            //interactIcon.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        //Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.tag == "Player"){
            //pl = null; //?!??!??? what is happening here why would we do this???????
            isClosest = false;
            playerInRange = false;
        }
    }
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
            nameplate.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + storedItem.GetComponent<Item>().name + "-\n-" + storedItem.GetComponent<Item>().value + "-";
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