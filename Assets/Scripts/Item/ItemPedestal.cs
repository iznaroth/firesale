using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public bool isPowerup;
    public bool pickingUp = false;

    public delegate void PowerupAcquireEvent();
    public static event PowerupAcquireEvent paEvent;

    public Slider buyItemSlider;
    public GameObject soldPrefab;
    
    void Awake(){
        PlayerController.interactEvent += PickUp;
        if(isPowerup){paEvent += disableOnOtherPickup;}
        StartCoroutine(animOffset());
        nameplate = GetComponentInChildren<Canvas>().gameObject;
        nameplate.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + storedItem.GetComponent<Item>().curioName + "-\n-" + storedItem.GetComponent<Item>().value + "-";
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

        if(isPowerup && pickingUp){
            buyItemSlider.value += 0.01f;
            if(buyItemSlider.value >= buyItemSlider.maxValue){
                this.buyPowerup();
            }
        } else if(isPowerup && !pickingUp && buyItemSlider.value > 0f){
            buyItemSlider.value -= 0.01f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if(col.gameObject.tag == "Player"){ //using name here is bad, use tags
            pl = col.gameObject.GetComponent<PlayerController>();
            string flexchar = isPowerup ? "- COST: $" : "-";
            nameplate.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + storedItem.GetComponent<Item>().curioName + "-\n" + flexchar + storedItem.GetComponent<Item>().value + "-";
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
        //Wrap these in subfunctions
        if(isPowerup){

        } else {
            if (pl != null && isClosest){
                Vector3 hold = this.storedItem.transform.position;
                this.storedItem.transform.position = toSwap.transform.position;
                toSwap.transform.position = hold;

                toSwap.transform.SetParent(this.gameObject.transform);
                this.storedItem.transform.SetParent(pl.gameObject.transform);

                pl.setHolding(this.storedItem);
                this.storedItem = toSwap;
                this.storedItem.transform.eulerAngles = new Vector3(0, 0, this.storedItem.GetComponent<Item>().spritePedestalRotation);
                nameplate.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "-" + storedItem.GetComponent<Item>().curioName + "-\n-" + storedItem.GetComponent<Item>().value + "-";
                //disable animation clip
            }
        }
    }

    public void OnDisable(){
        PlayerController.interactEvent -= PickUp;
    }

    public IEnumerator animOffset(){
        yield return new WaitForSeconds(startDelay);
        this.GetComponent<Animation>().Play();
    }

    public void disableOnOtherPickup(){
        Destroy(this.gameObject);
    }

    public void buyPowerup(){
        if(isPowerup){
            //Take powerup
            if(GameManager.currentIncome >= storedItem.GetComponent<Item>().value){
                switch(storedItem.GetComponent<Item>().curioName){
                    case "Grappling Hook":
                        this.pl.SetAbility(PlayerAbility.GRAPPLE_HOOK);
                        break;
                    case "Rocket Boost":
                        this.pl.SetAbility(PlayerAbility.ROCKET_BOOST);
                        break;
                }

                Debug.Log(GameManager.currentIncome);
                GameManager.currentIncome -= storedItem.GetComponent<Item>().value;
                Debug.Log(GameManager.currentIncome);
                //fire acquisition sfx
                paEvent?.Invoke(); //kill all other powerups
                Instantiate(soldPrefab, this.gameObject.transform.position, Quaternion.identity);
                disableOnOtherPickup();


            } else {
                //Fire failure sfx
            }
        }
    }

    public void flagPickup(bool flag){
        Debug.Log("Picking Up:" + flag);
        this.pickingUp = flag;
    }

}
