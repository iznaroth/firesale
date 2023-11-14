using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Flags]
public enum NPC_Types
{
    Any = 0, 
    AnimeFan = 1, 
    Anarchist = 2, 
    Capitalist = 4
}

public class Microgame_Base : MonoBehaviour
{
    public string microgameTitle;
    public NPC_Types npcType;
    public float microgameTimeLimit;
    public int microgameDifficulty; // difficulty of completion, used to determine if player has to do another microgame to convince the npc

    protected bool microgameWon = false;


    public delegate void WinCheck(bool result);
    public static event WinCheck winCheckEvent;


    public virtual bool SetupGame()
    {
        return false;
    }

    public virtual void StartGame()
    {
        
    }

    public virtual void EndGame()
    {
        winCheckEvent?.Invoke(microgameWon);
    }
    static IEnumerable<NPC_Types> GetFlags(NPC_Types input)
    {
        foreach (NPC_Types value in NPC_Types.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }
}
