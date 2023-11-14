using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject customerTextbox;  // these maybe just need to be the typewriter effects tbh
    public GameObject playerTextbox;    // these maybe just need to be the typewriter effects tbh
    public MicrogameManager microgameManager;
    public TextMeshProUGUI microgameStatusText;
    private Animator anime;

    private Microgame_Base currentMicrogame;
    public Slider healthbar;
    public TextMeshProUGUI moneyText;
    public static float PlayerHealth = 100;
    public static float PlayerIncome = 666;

    public string currentCurioName;
    public string[] playerBarks;

    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;
    public int customerChances = 2; //How many times the player can fail before taking damage
    public int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy
    public NPC_Types npcType = NPC_Types.AnimeFan;


    public bool animationDone = false;
    private bool microgameActive = false;

    public static bool wonLastMicrogame = false;
    public static bool newDialogueStarted = false;
    public static int dialogueState = 0;
    // Start is called before the first frame update
    void Start()
    {
        //customerTextbox.GetComponent<TypewriterEffect>().NewText(customerStartBarks[0]);
        anime = this.GetComponent<Animator>();
        this.transform.GetChild(0).gameObject.SetActive(false);
        microgameActive = false;
    }
    private void OnEnable()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        anime = this.GetComponent<Animator>();
        microgameActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (animationDone && !newDialogueStarted)
        {
            StartDialogue();
        }
        if (!microgameActive && !newDialogueStarted && dialogueState >= 2)
        {
            SummonMicrogame();
        }

        healthbar.value = PlayerHealth;
        moneyText.text = PlayerIncome.ToString("#,##0");
    }

    private void SummonMicrogame()
    {
        microgameManager.transform.GetChild(0).gameObject.SetActive(true);
        currentMicrogame = microgameManager.GetNewMicrogame(NPC_Types.AnimeFan);
        currentMicrogame.gameObject.SetActive(false);
        microgameManager.StartNewGame();
        Microgame_Base.winCheckEvent += MicrogameResult;
        microgameActive = true;
    }

    public void MicrogameResult(bool result)
    {
        Debug.Log("Result: " + result);
        Microgame_Base.winCheckEvent -= MicrogameResult;
        Destroy(currentMicrogame.gameObject);
        //microgameActive = false;
        microgameManager.GameResult(result);
        
    }
    public void StartDialogue()
    {
        newDialogueStarted = false;
        string currentDialogue ="";
        switch (dialogueState)
        {
            case 0:
                microgameManager.transform.GetChild(0).gameObject.SetActive(false);
                if (newDialogueStarted)
                {
                    currentDialogue = customerStartBarks[Random.Range(0, playerBarks.Length - 1)].Replace("{item}", "<i>" + currentCurioName + "</i>");
                    customerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                }
                else if(wonLastMicrogame && customerWinAmount > 0)
                {
                    currentDialogue = customerPositiveBarks[Random.Range(0, playerBarks.Length - 1)].Replace("{item}", "<i>" + currentCurioName + "</i>");
                    customerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                }
                else if (!wonLastMicrogame && customerWinAmount > 0)
                {
                    currentDialogue = customerNegativeBarks[Random.Range(0, playerBarks.Length - 1)].Replace("{item}", "<i>" + currentCurioName + "</i>");
                    customerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                }
                dialogueState++;
                break;
            case 1:
                currentDialogue = playerBarks[Random.Range(0, playerBarks.Length - 1)].Replace("{item}", "<i>" + currentCurioName + "</i>").ToUpper();
                playerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                dialogueState++;
                break;
            default:
                break;
        }
    }


    //play cut in stuff
    public void CutIn()
    {
        microgameActive = false;
        microgameManager.transform.GetChild(0).gameObject.SetActive(false);
        anime.SetTrigger("CutIn");
        anime.ResetTrigger("CutOut");
    }

    //play cut out stuff
    public void CutOut()
    {
        microgameActive = false;
        microgameManager.transform.GetChild(0).gameObject.SetActive(false);
        anime.SetTrigger("CutOut");
        anime.ResetTrigger("CutIn");
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
