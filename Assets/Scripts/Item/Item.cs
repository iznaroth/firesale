using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //Data class for Item representation.
    public string curioName;
    public int value;
    public Sprite sprite;
    public float spriteHoldRotation;
    public float spritePedestalRotation;

    private void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>().sprite;
    }
}
