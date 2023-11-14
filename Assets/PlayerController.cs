using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
        // assign the actions asset to this field in the inspector:
    // public InputActionAsset actions;

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
        moveAction = InputManager.GetInputAction(EInGameAction.MOVE);
        pickupAction = InputManager.GetInputAction(EInGameAction.PICK_UP);

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        pickupAction.performed += PickUp;

    }
    void Update()
    {
        // our update loop polls the "move" action value each frame
        // moveVector = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate(){
        //
        // float velX = Mathf.Clamp(body.velocity.x + (moveVector.x * accelRate), -speedLimit, speedLimit);
        // float velY = Mathf.Clamp(body.velocity.y + (moveVector.y * accelRate), -speedLimit, speedLimit);

        Vector2 vel = body.velocity;

        if (moveVector.sqrMagnitude < 0.1f)
		{
            float slowdownAmt = decelRate * Time.fixedDeltaTime;
            slowdownAmt = Mathf.Min(slowdownAmt, vel.magnitude);

            vel -= slowdownAmt * vel.normalized;
		}
        else
		{
            Vector2 accelDir = moveVector * speedLimit - vel;
            float accelAmt = accelRate * Time.fixedDeltaTime;
            
            vel += accelAmt * accelDir.normalized;
		}

        body.velocity = Vector2.ClampMagnitude(vel, speedLimit);

        /*
        if(moveVector.x == 0){

            velX += (decelRate * -velX);
        }
        if(moveVector.y == 0){
            velY += (decelRate * -velY);
        }
        */

        //Debug.Log(body.velocity);
    }

    void OnEnable()
    {
        // moveAction.Enable();
        // actions.FindActionMap("InGame").Enable();
    }
    void OnDisable()
    {
        // moveAction.Disable();
        // actions.FindActionMap("InGame").Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
	{
        moveVector = context.ReadValue<Vector2>();
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
