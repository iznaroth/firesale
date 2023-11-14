using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
        // assign the actions asset to this field in the inspector:
    public InputActionAsset actions;

    // private field to store move action reference
    private InputAction moveAction;

    [SerializeField]
    public float accelRate;
    public float decelRate;
    public Rigidbody2D body;
    private Vector2 moveVector = Vector2.zero;
    public float speedLimit;
    private Item holding;

    private InputAction pickupAction;
    public static event Action interactEvent;

    void Awake()
    {
        // find the "move" action, and keep the reference to it, for use in Update
        moveAction = actions.FindActionMap("InGame").FindAction("Move");
        pickupAction = actions.FindActionMap("InGame").FindAction("Pick Up");
        pickupAction.performed += PickUp;

    }
    void Update()
    {
        // our update loop polls the "move" action value each frame
        moveVector = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate(){
        //
        float velX = Mathf.Clamp(body.velocity.x + (moveVector.x * accelRate), -speedLimit, speedLimit);
        float velY = Mathf.Clamp(body.velocity.y + (moveVector.y * accelRate), -speedLimit, speedLimit);

        if(moveVector.x == 0){
            velX += (decelRate * -velX);
        }
        if(moveVector.y == 0){
            velY += (decelRate * -velY);
        }

        Vector2 vel = new Vector2(velX, velY);
        body.velocity = Vector2.ClampMagnitude(vel, speedLimit);
        //Debug.Log(body.velocity);
    }

    void OnEnable()
    {
        //moveAction.Enable();
        actions.FindActionMap("InGame").Enable();
    }
    void OnDisable()
    {
        //moveAction.Disable();
        actions.FindActionMap("InGame").Disable();
    }

    private void PickUp(InputAction.CallbackContext context){
        interactEvent?.Invoke();
    }

    public void setHolding(Item to){
        this.holding = to;
    }

    public Item getHolding(){
        return this.holding;
    }
}
