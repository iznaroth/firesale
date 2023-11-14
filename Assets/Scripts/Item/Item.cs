using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //Data class for Item representation.
    public Sprite sprite;
    public int curseVal;
    public int weightVal;

    public Item(Sprite nsprite, int nCurseVal, int nWeightVal){
        this.sprite = nsprite;
        this.curseVal = nCurseVal;
        this.weightVal = nWeightVal;
    }
}
