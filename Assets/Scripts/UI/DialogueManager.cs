using System.Collections;
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
    public Sprite[] customerImages;
    public Image handImage;
    public Image faceImage;
    public MicrogameManager microgameManager;
    public Slider healthbar;
    public TextMeshProUGUI moneyText;
    public Image gameTimer;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI CurioText;
    public TextMeshProUGUI NPC_Name;
    public TextMeshProUGUI AbilityName;
    public Slider AbilityCooldown;
    private Animator anime;
    private Microgame_Base currentMicrogame;
    private GameObject currentNPC;


    [Space(10)]
    [Header("Dialogue and Barks")]
    public string[] playerBarks;
    public string[] playerYells;
    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;
    public string[] customerGrappleStartBarks;
    public string[] customerRocketStartBarks;

    [Space(10)]
    [Header("Other Settings")]
    public Vector2 customerChancesRange = new Vector2(1, 2); //How many times the player can fail before taking damage
    public Vector2 customerDifficultyRange = new Vector2(1, 2);
    private int customerChances = 2; //How many times the player can fail before taking damage
    private int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy
    public int currencyCounterAnimationFrameRate = 30;
    public int currencyCounterAnimationMaxDuration = 2;
    public float handSpeed = 1f;
    public AudioClip startDialogueSound;


    private NPC_Types npcType = NPC_Types.AnimeFan;
    public Start_Conditions startCondition = Start_Conditions.Normal;
    [HideInInspector] public bool animationDone = false;

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
    public static UnityAction DialogueInteractionEnded;
    public static bool isThisEvenActive = false;

    private void Awake()
	{
		if (instance != null && instance != this)
		{
            Debug.LogWarning("Duplicate Dialogue Managers found, deleting " + name + "'s");
            Destroy(this);
            return;
		}

        dialogueState = 0;
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
        this.transform.GetChild(1).gameObject.SetActive(false);
        microgameActive = false;
    }
    private void OnEnable()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        anime = this.GetComponent<Animator>();
        microgameActive = false;
        microgameManager.transform.GetChild(0).transform.localScale = Vector3.zero;
        gameTimerText.fontSize = 40;
        gameTimerText.text = "";
        customerTextbox.GetComponent<TypewriterEffect>().fromDM = true;
        playerTextbox.GetComponent<TypewriterEffect>().fromDM = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isThisEvenActive"")

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

    private void LateUpdate()
    {
        if (isThisEvenActive)
        {
            //Debug.Log("Is MicrogameActive?: " + microgameActive + "\n is Animation Done?: " + animationDone + "\n is newDialogue Started?: " + newDialogueStarted + "\n CustomerWinAmount?: " + customerWinAmount + "\n Customer Chances?: " + customerChances + "\n Dialogue State?: " + dialogueState);
            if (!microgameActive && animationDone && !newDialogueStarted && customerWinAmount > 0 && customerChances > 0 && dialogueState < 2)
            {
                StartDialogue();

            }
            else if (!microgameActive && animationDone && !newDialogueStarted && dialogueState == 0 && (customerWinAmount <= 0 || customerChances <= 0))
            {
                CutOut();
            }
            else if (!microgameActive && !newDialogueStarted && dialogueState >= 2)
            {
                Debug.Log("Normal Microgame Summon");
                SummonMicrogame();
            }
            SetHandPosition();
        }
    }

    private void SummonMicrogame()
    {
        microgameActive = true;
        newDialogue = false;
        microgameManager.transform.GetChild(0).gameObject.SetActive(true);
        currentMicrogame = microgameManager.GetNewMicrogame(npcType);
        currentGameDifficulty = currentMicrogame.microgameDifficulty;
        currentGameDamage = currentMicrogame.microgameDamage;
        currentMicrogame.gameObject.SetActive(false);
        microgameManager.StartNewGame();
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
            GameManager.hpRemaining -= currentGameDamage;
        }
        if (currentMicrogame != null) { Destroy(currentMicrogame.gameObject); }
    }

    private void StartDialogue()
    {
        //Debug.Log("fucking what?");
        newDialogueStarted = true;
        string currentDialogue ="";
        switch (dialogueState)
        {
            case 0:
                playerTextbox.GetComponent<TextMeshProUGUI>().text = "";
                if (newDialogue)
                {
                    newDialogue = false;
                    if (startCondition == Start_Conditions.Grappled) { currentDialogue = customerGrappleStartBarks[Random.Range(0, customerGrappleStartBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>"); }
                    else if (startCondition == Start_Conditions.Rocketed) { currentDialogue = customerRocketStartBarks[Random.Range(0, customerRocketStartBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>"); }
                    else { currentDialogue = customerStartBarks[Random.Range(0, customerStartBarks.Length - 1)].Replace("{item}", "<i>" + currentCurio + "</i>"); }
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
            case 2:
                if (!microgameActive)
                {
                    Debug.Log("Fucked Microgame Summon");
                    microgameActive = true;
                    SummonMicrogame();
                }
                break;
            default:
                break;
        }
        Debug.Log(currentDialogue);
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
                gameTimerText.fontSize = 48;
            }
            else if (roundedTime <= 15 && roundedTime > 9)
            {
                gameTimerText.text += "!";
                gameTimerText.fontSize = 56;
            }
            else if (roundedTime <= 9 && roundedTime > 5)
            {
                gameTimerText.text = " " + roundedTime.ToString() + "!";
                gameTimerText.fontSize = 56 + ((10 - roundedTime) * 2);
            }
            else if (roundedTime <= 5 && roundedTime > 1)
            {
                gameTimerText.text = "" + roundedTime.ToString() + "!!";
                gameTimerText.fontSize = 56 + ((10 - roundedTime) * 4);
            }
            else if (roundedTime <= 1)
            {
                gameTimerText.text = " " + roundedTime.ToString() + "!!";
                gameTimerText.fontSize = 56 + ((9 - roundedTime) * 4);
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
        startCondition = Start_Conditions.Normal;
        anime.SetTrigger("CutOut");
        anime.ResetTrigger("CutIn");
        if (wonLastMicrogame)
        {
            currentNPC.GetComponent<SpeechBubble>().OpenSpeechBubble(customerSoldBarks[Random.Range(0, customerSoldBarks.Length)].Replace("{item}", "<i>" + currentCurio + "</i>"), 5);
        }
        else
        {
            currentNPC.GetComponent<SpeechBubble>().OpenSpeechBubble(customerRefusedBarks[Random.Range(0, customerRefusedBarks.Length)].Replace("{item}", "<i>" + currentCurio + "</i>"), 5);
        }
        PedestrianAI ped = currentNPC.GetComponent<PedestrianAI>();
        ped?.UnFreeze();
        PlayerController pc = GameManager.Player.GetComponent<PlayerController>();
        pc?.UnFreeze();

        InputManager.PopActionMap();
        DialogueInteractionEnded?.Invoke();

        if (wonLastMicrogame)
		{
            GameManager.IncreaseMoney();
            ped?.TakeItem(pc.RemoveItem());
            GameManager.curiosRemaining--;
            if (GameManager.curiosRemaining <= 0)
			{
                GameManager.instance.EndGame(true);
			}
        }
    }

    public void StartDialogueInteraction(GameObject newNPC, Start_Conditions wasStartedHow = Start_Conditions.Normal)
    {
        if (!isThisEvenActive) 
        {
            startCondition = wasStartedHow;
            GameManager.SpawnAudio(startDialogueSound, 1, 1, this.transform.position);
            PedestrianAI newPed = newNPC.GetComponent<PedestrianAI>();
            if(newPed != null)
            {
                customerTextbox.GetComponent<TypewriterEffect>().ChangeSoundSettings(newPed.speechSound, newPed.speechVolume, newPed.speechPitch, newPed.speechBasePitchRandomizationRange);
            }
            animationDone = false;
            wonLastMicrogame = false;
            microgameActive = false;
            newDialogue = true;
            customerTextbox.GetComponent<TextMeshProUGUI>().text = "";
            playerTextbox.GetComponent<TextMeshProUGUI>().text = "";
            customerTextbox.GetComponent<TypewriterEffect>().fromDM = true;
            playerTextbox.GetComponent<TypewriterEffect>().fromDM = true;
            customerChances = (int)Random.Range(customerChancesRange.x, customerChancesRange.y);
            customerWinAmount = (int)Random.Range(customerDifficultyRange.x, customerDifficultyRange.y);
            npcType = GetRandomEnum<NPC_Types>();
            PickRandomCustomerName();
            PickRandomCustomerPortrait();
            currentNPC = newNPC;
            dialogueState = 0;
            CutIn();
            InputManager.PushActionMap(EActionMap.MINIGAME);
        }
        else
        {
            Debug.Log("something tried to start a second dialogue");
        }
    }
/*    public void StartDialogueInteraction(GameObject newNPC)
    {
        if (!isThisEvenActive)
        {
            GameManager.SpawnAudio(startDialogueSound, 1, 1, this.transform.position);
            PedestrianAI newPed = newNPC.GetComponent<PedestrianAI>();
            if (newPed != null)
            {
                customerTextbox.GetComponent<TypewriterEffect>().ChangeSoundSettings(newPed.speechSound, newPed.speechVolume, newPed.speechPitch, newPed.speechPitchRandomizationRange);
            }
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
    }*/

    private void PickRandomCustomerPortrait()
    {
        customerImage.sprite = customerImages[Random.Range(0, customerImages.Length)];
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

        stepAmount = Mathf.Abs(stepAmount); //WARNING

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
                Debug.Log("Subtracting! " + stepAmount);
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

    public static void SetAbilityName(string newName)
	{
        instance.AbilityName.text = newName;
	}

    public static void SetAbilityCooldown(float fillAmt)
	{
        instance.AbilityCooldown.value = fillAmt;
	}
    public static bool IsInDialogue()
	{
        return DialogueManager.isThisEvenActive;
	}
    public void SetHandPosition()
    {
        Vector3 goalVector = (Input.mousePosition + faceImage.transform.position) / 20;
        handImage.transform.position = Vector3.Lerp(handImage.transform.position, goalVector, handSpeed * Time.deltaTime);
    }
}
