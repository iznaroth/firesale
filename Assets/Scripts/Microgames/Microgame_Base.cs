using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Microgame_Base : MonoBehaviour
{
    public string microgameTitle;
    public float microgameTimeLimit;
    public int microgameDifficulty; // difficulty of completion, used to determine if player has to do another microgame to convince the npc
    [HideInInspector] public bool microgameWon = false;

    // Start is called before the first frame update
    public virtual void StartGame()
    {
        
    }

    public virtual bool EndGame()
    {
        return false;
    }
}
