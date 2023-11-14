using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Microgame_Controller : MonoBehaviour
{
    public string microgameTitle;
    public float microgameTimeLimit;
    public int microgameDifficulty; // difficulty of completion, used to determine if player has to do another microgame to convince the npc
    protected bool microgameWon = false;

    public delegate void WinCheck(bool result);
    public static event WinCheck winCheckEvent;

    // Start is called before the first frame update
    public virtual void StartGame()
    {

    }

    public virtual void EndGame()
    {
        if (winCheckEvent != null)
        {
            winCheckEvent(microgameWon);
        }
        Destroy(this.gameObject);
    }
}
