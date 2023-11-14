using System.Collections;
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
    private Animator anime;
    private Microgame_Base currentMicrogame;


    public string currentCurioName;

    [Header("Dialogue and Barks")]
    public string[] playerBarks;
    public string[] customerStartBarks;
    public string[] customerSoldBarks;
    public string[] customerRefusedBarks;
    public string[] customerPositiveBarks;
    public string[] customerNegativeBarks;

    [Header("Gameplay Settings")]
    public Vector2 customerChancesRange = new Vector2(1, 2); //How many times the player can fail before taking damage
    public Vector2 customerDifficultyRange = new Vector2(1, 2);
    private int customerChances = 2; //How many times the player can fail before taking damage
    private int customerWinAmount = 2; //How many times the player needs to win to convince the customer to buy
    private NPC_Types npcType = NPC_Types.AnimeFan;
    [HideInInspector] public bool animationDone = false;
    private bool isThisEvenActive = false;

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
        currentMicrogame.gameObject.SetActive(false);
        microgameManager.StartNewGame();
        Microgame_Base.winCheckEvent += MicrogameResult;
        microgameActive = true;
    }

    public void MicrogameResult(bool result)
    {
        Microgame_Base.winCheckEvent -= MicrogameResult;
        if (result)
        {
            customerWinAmount -= currentMicrogame.microgameDifficulty;
        }
        Destroy(currentMicrogame.gameObject);
        microgameManager.GameResult(result);

        
    }
    private void StartDialogue()
    {
        string currentDialogue ="";
        switch (dialogueState)
        {
            case 0:
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
        isThisEvenActive = true;
        anime.ResetTrigger("CutOut");
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

    public void StartDialogueInteraction()
    {
        customerChances = (int)Random.Range(customerChancesRange.x, customerChancesRange.y);
        customerWinAmount = (int)Random.Range(customerDifficultyRange.x, customerDifficultyRange.y);
        npcType = GetRandomEnum<NPC_Types>();
        PickRandomCustomerPortrait();
        Debug.Log(npcType);
        CutIn();
    }

    private void PickRandomCustomerPortrait()
    {

    }
    public static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(1, A.Length));
        return V;
    }
}
