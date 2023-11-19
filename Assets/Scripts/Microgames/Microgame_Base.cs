using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Flags]
public enum NPC_Types
{
    AnimeFan = 1, 
    RichFella = 2, 
    Intellectual = 4,
    Gamer = 8,
    InternetPoster = 16,
    SmallChild = 32,
    CrystalMommy = 64,
    FineArtEnjoyer = 128,
    LanguageEnjoyer = 256,
    MusicFan = 512,
    Streamers = 1024,
    Deadbeat = 2048,
    FanficWriter = 4096,
    SalaryMan = 8192,
    Edgelord = 16384,
    SportsFan = 32768,
    StepDad = 65536,
    MadScientist = 131072
}

public class Microgame_Base : MonoBehaviour
{
    [Header("Microgame Base Variables")]
    public string microgameTitle;               // Title of the Microgame
    public NPC_Types npcType;                   // which type of npc can call this microgame
    public float microgameStartDelay = 1.5f;    // delay before microgame starts 
    public float microgameTimeLimit;            // How long the player has to complete the microgame
    public int microgameDifficulty;             // difficulty of completion, used to determine if player has to do another microgame to convince the npc
    public int microgameDamage = 10;          // Damage for losing this microgame
    protected bool microgameWon = false;        // if microgame was won or lost

    //Event stuff for automating handling and all that
    public delegate void WinCheck(bool result);
    public static event WinCheck winCheckEvent;

    //if the game needs intial setup this is called when it's spawned
    public virtual bool SetupGame()
    {
        return false;
    }
    
    //this is for when the game actually starts
    public virtual void StartGame()
    {
        
    }

    //this is for when the game ends (while set result of microgame to current microgameWon bool)
    public virtual void EndGame()
    {
        winCheckEvent?.Invoke(microgameWon);
    }
}
