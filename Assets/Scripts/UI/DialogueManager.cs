﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Object Connections")]
    public GameObject customerTextbox;  // these maybe just need to be the typewriter effects tbh
    public GameObject playerTextbox;    // these maybe just need to be the typewriter effects tbh
    public Image customerImage;
    public MicrogameManager microgameManager;
    public Slider healthbar;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI NPC_Name;
    private Animator anime;
    private Microgame_Base currentMicrogame;
    [HideInInspector] private string currentCurioName;


    [Space(10)]
    [Header("Dialogue and Barks")]
    public string[] playerBarks;
    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;

    [Space(10)]
    [Header("Gameplay Settings")]
    public Vector2 customerChancesRange = new Vector2(1, 2); //How many times the player can fail before taking damage
    public Vector2 customerDifficultyRange = new Vector2(1, 2);
    private int customerChances = 2; //How many times the player can fail before taking damage
    private int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy
    private NPC_Types npcType = NPC_Types.AnimeFan;
    [HideInInspector] public bool animationDone = false;
    private bool isThisEvenActive = false;

    //weird emergency cache stuff to avoid errors
    private int currentGameDifficulty = 0;

    public static float PlayerHealth = 100;
    public static float PlayerIncome = 666;
    public static bool microgameActive = false;
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
        microgameManager.transform.GetChild(0).transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (isThisEvenActive)
        {
            if (animationDone && !newDialogueStarted && customerWinAmount > 0 && customerChances > 0)
            {
                StartDialogue();
            }
            else if (animationDone && !newDialogueStarted && dialogueState == 0 && (customerWinAmount <= 0 || customerChances <= 0))
            {
                CutOut();
            }

            if (!microgameActive && !newDialogueStarted && dialogueState >= 2)
            {
                SummonMicrogame();
            }
        }

        healthbar.value = PlayerHealth;
        moneyText.text = PlayerIncome.ToString("#,##0");
    }

    private void SummonMicrogame()
    {
        microgameManager.transform.GetChild(0).gameObject.SetActive(true);
        currentMicrogame = microgameManager.GetNewMicrogame(npcType);
        currentGameDifficulty = currentMicrogame.microgameDifficulty;
        currentMicrogame.gameObject.SetActive(false);
        microgameManager.StartNewGame();
        microgameActive = true;
    }

    public void MicrogameResult()
    {
        if (wonLastMicrogame)
        {
            customerWinAmount -= currentGameDifficulty;
        }
        else
        {
            customerChances -= currentGameDifficulty;
        }
        Destroy(currentMicrogame.gameObject);
    }

    private void StartDialogue()
    {
        string currentDialogue ="";
        switch (dialogueState)
        {
            case 0:
                playerTextbox.GetComponent<TextMeshProUGUI>().text = "";
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
    private void CutIn()
    {
        microgameActive = false;
        microgameManager.transform.GetChild(0).gameObject.SetActive(false);
        anime.SetTrigger("CutIn");
        anime.ResetTrigger("CutOut");
        isThisEvenActive = true;
        animationDone = false;
    }

    //play cut out stuff
    private void CutOut()
    {
        isThisEvenActive = false;
        microgameActive = false;
        microgameManager.transform.GetChild(0).gameObject.SetActive(false);
        anime.SetTrigger("CutOut");
        anime.ResetTrigger("CutIn");
    }

    public void StartDialogueInteraction(string currentCurio)
    {
        if (!isThisEvenActive) 
        { 
            animationDone = false;
            wonLastMicrogame = false;
            microgameActive = false;
            customerTextbox.GetComponent<TextMeshProUGUI>().text = "";
            playerTextbox.GetComponent<TextMeshProUGUI>().text = "";
            customerChances = (int)Random.Range(customerChancesRange.x, customerChancesRange.y);
            customerWinAmount = (int)Random.Range(customerDifficultyRange.x, customerDifficultyRange.y);
            npcType = GetRandomEnum<NPC_Types>();
            PickRandomCustomerName();
            PickRandomCustomerPortrait();
            currentCurioName = currentCurio;
            CutIn();
        }
    }

    private void PickRandomCustomerPortrait()
    {

    }

    private void PickRandomCustomerName()
    {
        int which_varient = Random.Range(0,2);
        switch (npcType)
        {
            case NPC_Types.AnimeFan:
                if (which_varient == 0) { NPC_Name.text = "Anime Nerd"; }
                else if (which_varient == 1) { NPC_Name.text = "Mangakan"; }
                break;
            case NPC_Types.RichFella:
                NPC_Name.text = "Rich Fella";
                break;
            case NPC_Types.Intellectual:
                if (which_varient == 0) { NPC_Name.text = "Self Proclaimed \" Intellectual\""; }
                else if(which_varient == 1) { NPC_Name.text = "Debate Pervert"; }
                break;
            case NPC_Types.Gamer:
                if (which_varient == 0) { NPC_Name.text = "Gamer\n(Derogatory)"; }
                else if (which_varient == 1) { NPC_Name.text = "Person of Play"; }
                break;
            case NPC_Types.InternetPoster:
                NPC_Name.text = "Internet Shitposter";
                break;
            case NPC_Types.SmallChild:
                NPC_Name.text = "Small... Child?";
                break;
            case NPC_Types.CrystalMommy:
                NPC_Name.text = "Crystal Mommy";
                break;
            case NPC_Types.FineArtEnjoyer:
                if (which_varient == 0) { NPC_Name.text = "Fine Arts Enjoyer"; }
                else if(which_varient == 1)  { NPC_Name.text = "Fine Arts Snob"; }
                break;
            case NPC_Types.LanguageEnjoyer:
                NPC_Name.text = "Polyglot";
                break;
            case NPC_Types.MusicFan:
                NPC_Name.text = "Music Fan";
                break;
            case NPC_Types.Streamers:
                NPC_Name.text = "1k Andy";
                break;
            case NPC_Types.Deadbeat:
                NPC_Name.text = "Prospectless Deadbeat";
                break;
            case NPC_Types.FanficWriter:
                NPC_Name.text = "Fanfic Writer";
                break;
            case NPC_Types.SalaryMan:
                NPC_Name.text = "Salaryman";
                break;
            case NPC_Types.Edgelord:
                NPC_Name.text = "r/niceguys Edgelord";
                break;
            case NPC_Types.SportsFan:
                NPC_Name.text = "SPORTS FANATIC";
                break;
            case NPC_Types.StepDad:
                NPC_Name.text = "Someone's\nStep-Dad";
                break;
            case NPC_Types.MadScientist:
                if(which_varient == 0) { NPC_Name.text = "Mad Scientist"; }
                else if(which_varient == 1)  { NPC_Name.text = "Doofenshmirtz?"; }
                break;
            default:
                break;
        }
    }

    public static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(1, A.Length));
        return V;
    }
}
