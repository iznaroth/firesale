using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject customerTextbox;  // these maybe just need to be the typewriter effects tbh
    public GameObject playerTextbox;    // these maybe just need to be the typewriter effects tbh
    public GameObject microgameBox;     

    public string currentCurioName;
    public string[] playerBarks;

    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;
    public int customerChances = 2; //How many times the player can fail before taking damage
    public int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy

    private bool microgameWon = false;
    public static bool playerCanRespond = false;
    private bool playerAlreadyResponded = false;


    // Start is called before the first frame update
    void Start()
    {
        //customerTextbox.GetComponent<TypewriterEffect>().NewText(customerStartBarks[0]);
    }
    private void OnEnable()
    {
        customerTextbox.GetComponent<TypewriterEffect>().NewText(customerStartBarks[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCanRespond && !playerAlreadyResponded)
        {
            playerAlreadyResponded = true;
            playerTextbox.GetComponent<TypewriterEffect>().NewText(playerBarks[0].ToUpper());
        }
        if(playerCanRespond && playerAlreadyResponded)
        {
            SummonMicrogame();
        }
    }

    private void SummonMicrogame()
    {
        microgameBox.SetActive(true);
    }

    public static void ForceHide()
    {
        //hide this object instantly
    }


    //play cut in stuff
    public void CutIn()
    {

    }

    //play cut out stuff
    public void CutOut()
    {

    }

    private void TypeText()
    {

    }
    private void PickRandomCustomerPortrait()
    {

    }

    //Take in a bark type and pick from a file of barks
    private string PickRandomCustomerBark()
    {
        return null;
    }

    //return a random new bark before reseting the availible list to avoid overlaps
    private string PickRandomPlayerBark()
    {
        //Make sure to return as all caps
        return "THE MUST HAVE AMNESIA BECAUSE THE FORGOT I'M HIM!";
    }
}
