using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI customerTextbox;
    public TextMeshProUGUI playerTextbox;
    public TextMeshProUGUI microgameTextbox;

    public string currentCurioName;
    public string[] playerBarks;

    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;
    public int customerChances = 2; //How many times the player can fail before taking damage
    public int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy

    public GameObject microgameObject;
    private float microgameTitle;
    private float microgameTimeLimit;
    private bool microgameWon = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SummonMinigame()
    {

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
