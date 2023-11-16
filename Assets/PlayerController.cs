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
    public float turnAssist;
    public int turnThreshold;
    public Rigidbody2D body;
    private Vector2 moveVector = Vector2.zero;
    public float speedLimit;
    public GameObject holding;

    private InputAction pickupAction;

    public delegate void InteractEvent(GameObject payload);
    public static event InteractEvent interactEvent;

    public Transform holdAnchor;


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
        float applyTurnaround = 0f;

        if(Mathf.Abs(Vector2.Angle(moveVector, vel)) > turnThreshold){
            applyTurnaround = turnAssist;
        }

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
            
            vel += ((accelAmt + applyTurnaround) * accelDir.normalized) ;
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

    private void OnMove(InputAction.CallbackContext context)
	{
        moveVector = context.ReadValue<Vector2>();
	}

    private void PickUp(InputAction.CallbackContext context){
        interactEvent?.Invoke(holding);
    }

    public void setHolding(GameObject to){
        this.holding = to;
        to.GetComponent<Animation>().playAutomatically = false;
        to.GetComponent<Animation>().Stop();
        to.transform.position = holdAnchor.position;
        to.transform.rotation = new Quaternion(0, 0, 45, 0);
        DialogueManager.currentCurio = this.holding.GetComponent<Item>().name;
    } 
}
