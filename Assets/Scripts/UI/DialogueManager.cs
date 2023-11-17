﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public enum Start_Conditions
{
    Normal,
    Grappled,
    Rocketed
}

public class DialogueManager : MonoBehaviour
{
    [Header("Object Connections")]
    public GameObject customerTextbox;  // these maybe just need to be the typewriter effects tbh
    public GameObject playerTextbox;    // these maybe just need to be the typewriter effects tbh
    public Image customerImage;
    public MicrogameManager microgameManager;
    public Slider healthbar;
    public TextMeshProUGUI moneyText;
    public Image gameTimer;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI CurioText;
    public TextMeshProUGUI NPC_Name;
    private Animator anime;
    private Microgame_Base currentMicrogame;
    private GameObject currentNPC;


    [Space(10)]
    [Header("Dialogue and Barks")]
    public string[] playerBarks;
    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;

    [Space(10)]
    [Header("Other Settings")]
    public Vector2 customerChancesRange = new Vector2(1, 2); //How many times the player can fail before taking damage
    public Vector2 customerDifficultyRange = new Vector2(1, 2);
    private int customerChances = 2; //How many times the player can fail before taking damage
    private int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy
    public int currencyCounterAnimationFrameRate = 30;
    public int currencyCounterAnimationMaxDuration = 2;
    private NPC_Types npcType = NPC_Types.AnimeFan;
    [HideInInspector] public bool animationDone = false;
    private bool isThisEvenActive = false;

    //weird emergency cache stuff to avoid errors
    private int currentGameDifficulty = 0;
    private int currentGameDamage = 0;
    private bool newDialogue = true;
    private bool changingIncomeValue = false;
    private int currentTrackedIncome = 0;

    public static DialogueManager instance;

    public static bool microgameActive = false;
    public static bool wonLastMicrogame = false;
    public static bool newDialogueStarted = false;
    public static string currentCurio = "Poggers";
    public static int dialogueState = 0;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
            Debug.LogWarning("Duplicate Dialogue Managers found, deleting " + name + "'s");
            Destroy(this);
            return;
		}

        instance = this;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
            instance = null;
		}
	}

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
        gameTimerText.fontSize = 56;
        gameTimerText.text = "";
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

        healthbar.value = GameManager.hpRemaining;
        if(!changingIncomeValue && currentTrackedIncome != GameManager.currentIncome)
        {
            changingIncomeValue = true;
            StartCoroutine("IncomeValueChanger");
        }
        gameTimer.fillAmount = GameManager.timeRemaining / GameManager.totalTime;
        CurioText.text = GameManager.curiosRemaining.ToString();
        WarningTimer();
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
        if (currentMicrogame != null) { Destroy(currentMicrogame.gameObject); }
    }

    private void StartDialogue()
    {
        string currentDialogue ="";
        switch (dialogueState)
        {
            case 0:
                playerTextbox.GetComponent<TextMeshProUGUI>().text = "";
                if (newDialogue)
                {
                    newDialogue = false;
                    currentDialogue = customerStartBarks[Random.Range(0, customerStartBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>");
                    customerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                }
                else if(wonLastMicrogame && customerWinAmount > 0)
                {
                    currentDialogue = customerPositiveBarks[Random.Range(0, customerPositiveBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>");
                    customerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                }
                else if (!wonLastMicrogame && customerWinAmount > 0)
                {
                    currentDialogue = customerNegativeBarks[Random.Range(0, customerNegativeBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>");
                    customerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                }
                dialogueState++;
                break;
            case 1:
                currentDialogue = playerBarks[Random.Range(0, playerBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>").ToUpper();
                playerTextbox.GetComponent<TypewriterEffect>().NewText(currentDialogue);
                dialogueState++;
                break;
            default:
                break;
        }
    }

    private void WarningTimer()
    {
        float roundedTime = Mathf.Round(GameManager.timeRemaining) + 1;
        if (roundedTime <= 60)
        {
            gameTimerText.text = roundedTime.ToString();
            if (roundedTime <= 30 && roundedTime > 15)
            {
                gameTimerText.text += "";
                gameTimerText.fontSize = 64;
            }
            else if (roundedTime <= 15 && roundedTime > 9)
            {
                gameTimerText.text += "!";
                gameTimerText.fontSize = 72;
            }
            else if (roundedTime <= 9 && roundedTime > 5)
            {
                gameTimerText.text = " " + roundedTime.ToString() + "!";
                gameTimerText.fontSize = 72 + ((10 - roundedTime) * 4);
            }
            else if (roundedTime <= 5)
            {
                gameTimerText.text = " " + roundedTime.ToString() + "!!";
                gameTimerText.fontSize = 72 + ((9 - roundedTime) * 6);
            }
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
        if (wonLastMicrogame)
        {
            
            currentNPC.GetComponent<SpeechBubble>().OpenSpeechBubble(customerSoldBarks[Random.Range(0, customerSoldBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>"), 5);
        }
        else
        {
            currentNPC.GetComponent<SpeechBubble>().OpenSpeechBubble(customerRefusedBarks[Random.Range(0, customerRefusedBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>"), 5);
        }
        currentNPC.GetComponent<PedestrianAI>()?.UnFreeze();
        GameManager.Player.GetComponent<PlayerController>()?.UnFreeze();
        InputManager.PopActionMap();
    }

    public void StartDialogueInteraction(GameObject newNPC)
    {
        if (!isThisEvenActive) 
        { 
            animationDone = false;
            wonLastMicrogame = false;
            microgameActive = false;
            newDialogue = true;
            customerTextbox.GetComponent<TextMeshProUGUI>().text = "";
            playerTextbox.GetComponent<TextMeshProUGUI>().text = "";
            customerChances = (int)Random.Range(customerChancesRange.x, customerChancesRange.y);
            customerWinAmount = (int)Random.Range(customerDifficultyRange.x, customerDifficultyRange.y);
            npcType = GetRandomEnum<NPC_Types>();
            PickRandomCustomerName();
            PickRandomCustomerPortrait();
            currentNPC = newNPC;
            CutIn();
            InputManager.PushActionMap(EActionMap.MINIGAME);
        }
    }

    private void PickRandomCustomerPortrait()
    {

    }

    private void PickRandomCustomerName()
    {
        int which_variant = Random.Range(0,2);
        switch (npcType)
        {
            case NPC_Types.AnimeFan:
                if (which_variant == 0) { NPC_Name.text = "Anime Nerd"; }
                else if (which_variant == 1) { NPC_Name.text = "Mangakan"; }
                break;
            case NPC_Types.RichFella:
                NPC_Name.text = "Rich Fella";
                break;
            case NPC_Types.Intellectual:
                if (which_variant == 0) { NPC_Name.text = "Self Proclaimed \" Intellectual\""; }
                else if(which_variant == 1) { NPC_Name.text = "Debate Pervert"; }
                break;
            case NPC_Types.Gamer:
                if (which_variant == 0) { NPC_Name.text = "Gamer\n(Derogatory)"; }
                else if (which_variant == 1) { NPC_Name.text = "Person of Play"; }
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
                if (which_variant == 0) { NPC_Name.text = "Fine Arts Enjoyer"; }
                else if(which_variant == 1)  { NPC_Name.text = "Fine Arts Snob"; }
                break;
            case NPC_Types.LanguageEnjoyer:
                NPC_Name.text = "Polyglot";
                break;
            case NPC_Types.MusicFan:
                NPC_Name.text = "Music Fan";
                break;
            case NPC_Types.Streamers:
                if (which_variant == 0) { NPC_Name.text = "Local Funny Man"; }
                else if (which_variant == 1) { NPC_Name.text = "\" Influencer\""; }
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
                NPC_Name.text = "SPORTS \nFANATIC";
                break;
            case NPC_Types.StepDad:
                NPC_Name.text = "Someone's\nStep-Dad";
                break;
            case NPC_Types.MadScientist:
                if(which_variant == 0) { NPC_Name.text = "Mad Scientist"; }
                else if(which_variant == 1)  { NPC_Name.text = "Doofenshmirtz?"; }
                break;
            default:
                break;
        }
    }

    private IEnumerator IncomeValueChanger()
    {
        WaitForSeconds Wait = new WaitForSeconds(1/currencyCounterAnimationFrameRate);
        int currentVal = GameManager.currentIncome;
        int stepAmount;

        if (currentVal - currentTrackedIncome < 0)
        {
            stepAmount = Mathf.FloorToInt((currentVal - currentTrackedIncome) / (currencyCounterAnimationFrameRate * currencyCounterAnimationMaxDuration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((currentVal - currentTrackedIncome) / (currencyCounterAnimationFrameRate * currencyCounterAnimationMaxDuration));
        }

        if(currentTrackedIncome < currentVal)
        {
            while(currentTrackedIncome < currentVal)
            {
                currentTrackedIncome += stepAmount;
                if(currentTrackedIncome > currentVal)
                {
                    currentTrackedIncome = currentVal;
                }
                moneyText.text = currentTrackedIncome.ToString("#,##0");
                yield return Wait;
            }
        }
        else
        {
            while (currentTrackedIncome > currentVal)
            {
                currentTrackedIncome -= stepAmount;
                if (currentTrackedIncome < currentVal)
                {
                    currentTrackedIncome = currentVal;
                }
                moneyText.text = currentTrackedIncome.ToString("#,##0");
                yield return Wait;
            }
        }
        currentTrackedIncome = currentVal;
        changingIncomeValue = false;
    }

    public static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(1, A.Length));
        return V;
    }
}
