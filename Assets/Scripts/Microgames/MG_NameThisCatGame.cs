using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_NameThisCatGame : Microgame_Base
{
    public override bool SetupGame()
    {
        // draw a cat pick from a array and set it to the cat we want
        return false;
    }

    public override void StartGame()
    {
        //spawn input field
    }

    private void Update()
    {
        //check if player hits enter or clicks submit or something
    }

    public virtual void EndGame()
    {
        base.EndGame();
    }
}
