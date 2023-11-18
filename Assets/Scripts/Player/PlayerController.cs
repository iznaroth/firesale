using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum PlayerAbility
{
    YELL,
    GRAPPLE_HOOK,
    ROCKET_BOOST,
}

public enum FacingDir
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}

public class PlayerController : MonoBehaviour
{
    // assign the actions asset to this field in the inspector:
    // public InputActionAsset actions;

    // private field to store move action reference
    private InputAction moveAction;

    [Header("Controls Variables")]
    public float speedLimit;
    public float accelRate;
    public float decelRate;
    public float turnAssist;
    public int turnThreshold;
    public float crashDisableTime = 0.5f;
    public LayerMask itemLayerMask;
    public float itemPickupX = 1.5f;
    public float itemPickupY = 2.5f;
    public float maxZoomOut = 2.5f;
    public float shopSpeedLimit;
    public float shopAccelRate;
    public float shopDecelRate;
    public float idleInputDeadzone = 0.1f;
    public float rocketBoostDuration = 5f;
    public float rocketBoostCooldown = 10f;
    public float rocketBoostMaxSpeed = 40f;
    public float rocketBoostAccel = 40f;
    public GameObject rocketBoostCanvas;
    public Slider boostSlider;
    public ParticleSystem flameParticles;

    [Header("Physics Variables")]
    [Range(0f, 1f)] public float bouncinessEnableThreshhold = 0.4f; // what percentage of max speed do we need to reach start to increase bounciness
    [Range(0f, 1f)] public float baseBounciness = 0.3f;
    [Range(0f, 1f)] public float maxBounciness = 0.63f;
    [Range(0f, 1f)] public float thudSoundThreshhold = 0.3f; // what percentage of max speed do we need to reach to play the thud sound
    [Range(0f, 1f)] public float thudSoundBaseVolume = 0.1f;
    [Range(0f, 1f)] public float thudSoundMaxVolume = 1; 
    public float thudSoundPitchRandomRange = 0.65f;

    public PlayerAbility defaultAbility = PlayerAbility.YELL;
    public float playerYellDuration = 5f;
    public float playerYellCooldown = 6f;

    private Rigidbody2D body;
    private AudioSource audioSource;
    private Vector2 moveVector = Vector2.zero;
    private GameObject holding;
    private bool cantMove = false;
    private PhysicsMaterial2D physMat;
    private GameObject closestItem;
    private PlayerAbility currentAbility = PlayerAbility.YELL;
    private AvoidPoint pedRepulsor;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private InputAction pickupAction;

    public delegate void InteractEvent(GameObject payload);
    public static event InteractEvent interactEvent;

    public Transform holdAnchor;
    public CinemachineVirtualCamera camera;
    public UnityAction<FacingDir> OnPlayerChangeDirection;

    public static bool inShop = false;

    private static Dictionary<PlayerAbility, string> PlayerAbilityDisplayNames = new Dictionary<PlayerAbility, string>
    {
        { PlayerAbility.YELL, "YELL" },
        { PlayerAbility.ROCKET_BOOST, "ROCKET BOOTS" },
        { PlayerAbility.GRAPPLE_HOOK, "GRAPPLE HOOK" },

    };

    private static Dictionary<FacingDir, Vector2> Directions = new Dictionary<FacingDir, Vector2>
    {
        { FacingDir.DOWN, Vector2.down },
        { FacingDir.LEFT, Vector2.left },
        { FacingDir.RIGHT, Vector2.right },
        { FacingDir.UP, Vector2.up },

    };

    bool frozen = false;
    bool rocketBoosting = false;
    float timeToNextRocketBoost;
    float rocketBoostRemaining;
    float timeToNextYell;
    Vector2 rocketBoostDir;
    FacingDir playerDirection = FacingDir.DOWN;
/*    bool m_Started = false;*/

	private void Awake()
	{
        GameManager.Player = this.gameObject;
        holding = this.GetComponentInChildren<Item>().gameObject;
	}

	void Start()
    {
        // find the "move" action, and keep the reference to it, for use in Update
        moveAction = InputManager.GetInputAction(EInGameAction.MOVE);
        pickupAction = InputManager.GetInputAction(EInGameAction.PICK_UP);
        body = this.GetComponent<Rigidbody2D>();
        physMat = body.sharedMaterial;
        audioSource = this.GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rocketBoostCanvas.SetActive(false);
        flameParticles.Stop();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        pickupAction.performed += PickUp;
        InputManager.GetInputAction(EInGameAction.ABILITY).started += OnAbility;

        pedRepulsor = GetComponent<AvoidPoint>();

        SetAbility(defaultAbility);

        //m_Started = true;
    }
    void Update()
    {
        // our update loop polls the "move" action value each frame
        // moveVector = moveAction.ReadValue<Vector2>();

        UpdateAnimatorBools();

        float charge;
        switch (currentAbility)
		{
            case PlayerAbility.GRAPPLE_HOOK:
                DialogueManager.SetAbilityCooldown(GrappleHookController.instance.GetNormallizedCharge());
                break;
            case PlayerAbility.YELL:
                charge = 1f;
                if (Time.time < timeToNextYell)
				{
                    charge -= (timeToNextYell - Time.time) / playerYellCooldown;
				}
                DialogueManager.SetAbilityCooldown(charge);
                break;
            case PlayerAbility.ROCKET_BOOST:
                charge = 1f;
                if (Time.time < timeToNextRocketBoost)
				{
                    charge -= (timeToNextRocketBoost - Time.time) / rocketBoostCooldown;
				}
                else if (rocketBoosting)
				{
                    charge = 0f;
				}
                DialogueManager.SetAbilityCooldown(charge);
                break;
        }
    }

    public void Freeze()
	{
        if (frozen) return;

        body.velocity = Vector2.zero;
        body.simulated = false;
        moveVector = Vector2.zero;

        frozen = true;
	}

    public void UnFreeze()
	{
        if (!frozen) return;

        body.simulated = true;

        frozen = false;
	}

    private void FixedUpdate(){
        //
        // float velX = Mathf.Clamp(body.velocity.x + (moveVector.x * accelRate), -speedLimit, speedLimit);
        // float velY = Mathf.Clamp(body.velocity.y + (moveVector.y * accelRate), -speedLimit, speedLimit);
        if (inShop)
        {
            NearestItemCheck();
        }

        if (!cantMove && rocketBoosting && !frozen)
		{
            /*Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 forceDir = mousePos - (Vector2)transform.position;
            forceDir.Normalize();*/
            if (moveVector.normalized.magnitude > 0.9f)
			{
                rocketBoostDir = moveVector.normalized;
            }

            if (rocketBoostDir.magnitude < 0.9f)
			{
                rocketBoostDir = Directions[playerDirection];
			}

            Vector2 vel = body.velocity;
            vel += rocketBoostDir * rocketBoostAccel * Time.deltaTime;
            vel = Vector2.ClampMagnitude(vel, rocketBoostMaxSpeed);

            body.velocity = vel;

            rocketBoostRemaining -= Time.deltaTime;
            if (rocketBoostRemaining < 0f)
			{
                rocketBoosting = false;
                timeToNextRocketBoost = Time.time + rocketBoostCooldown;
                rocketBoostCanvas.SetActive(false);
                flameParticles.Stop();
			}
            else
			{
                boostSlider.value = rocketBoostRemaining / rocketBoostDuration;
			}
		}
        else if (!cantMove)
        {
            Vector2 vel = body.velocity;
            float applyTurnaround = 0f;

            if (Mathf.Abs(Vector2.Angle(moveVector, vel)) > turnThreshold)
            {
                applyTurnaround = turnAssist * Time.deltaTime;
            }

            if (moveVector.sqrMagnitude < 0.1f)
            {
                float slowdownAmt = (inShop? shopDecelRate : decelRate) * Time.deltaTime; // fixed wrong delta time, shouldn't have used fixedDeltaTime like that
                slowdownAmt = Mathf.Min(slowdownAmt, vel.magnitude);

                vel -= slowdownAmt * vel.normalized;
            }
            else
            {
                Vector2 accelDir = moveVector * (inShop ? shopSpeedLimit : speedLimit) - vel;
                float accelAmt = (inShop ? shopAccelRate : accelRate) * Time.deltaTime;

                vel += ((accelAmt + applyTurnaround) * accelDir.normalized);
            }

            body.velocity = Vector2.ClampMagnitude(vel, (inShop ? shopSpeedLimit : speedLimit));

            if ((vel.magnitude / speedLimit) >= bouncinessEnableThreshhold)
            {
                physMat.bounciness = maxBounciness * (vel.magnitude / speedLimit);
            }

            if ((vel.magnitude / speedLimit) >= thudSoundThreshhold)
            {
                audioSource.volume = thudSoundMaxVolume * (vel.magnitude / speedLimit);
            }
            else
            {
                audioSource.volume = thudSoundBaseVolume;
            }

            if (!inShop) { }// add camera zoom out here 
        }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((body.velocity.magnitude / speedLimit) >= thudSoundThreshhold && !inShop) // player crashed at a high speed, disable controls to make them bounce
        {
            cantMove = true;
            StartCoroutine("MoveDelay");
            moveVector = Vector2.zero;
        }
        body.sharedMaterial = physMat;
        audioSource.pitch = 1 + Random.Range(-thudSoundPitchRandomRange, thudSoundPitchRandomRange);
        if(collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = body.velocity / 0.25f;
        }
    }


    private void OnMove(InputAction.CallbackContext context)
	{
        Vector2 input = context.ReadValue<Vector2>();

        if (!cantMove) 
        {
            input.Normalize();
            moveVector = input;
        }
    }

    void UpdateAnimatorBools()
	{
        Vector2 dir = rocketBoosting ? rocketBoostDir : moveVector;

        animator.SetBool("hasItem", IsHolding());
        animator.SetBool("isIdle", frozen || dir.magnitude < idleInputDeadzone);

        if (dir.y > 0.5f)
        {
            animator.SetBool("up", true);
            animator.SetBool("down", false);
            animator.SetBool("right", false);
            SetFacingDir(FacingDir.UP);
        }
        else if (dir.y < -0.5f)
        {
            animator.SetBool("up", false);
            animator.SetBool("down", true);
            animator.SetBool("right", false);
            SetFacingDir(FacingDir.DOWN);
        }
        else if (dir.x > 0.5f)
        {
            animator.SetBool("up", false);
            animator.SetBool("down", false);
            animator.SetBool("right", true);
            spriteRenderer.flipX = false;
            SetFacingDir(FacingDir.RIGHT);
        }
        else if (dir.x < -0.5f)
        {
            animator.SetBool("up", false);
            animator.SetBool("down", false);
            animator.SetBool("right", true);
            spriteRenderer.flipX = true;
            SetFacingDir(FacingDir.LEFT);
        }
    }

    void SetFacingDir(FacingDir newDir)
	{
        if (newDir == playerDirection) return;

        playerDirection = newDir;
        OnPlayerChangeDirection?.Invoke(playerDirection);
	}

    private void OnAbility(InputAction.CallbackContext context)
	{
		switch (currentAbility)
		{
			case PlayerAbility.YELL:
                if (Time.time < timeToNextYell) return;

                SpeechBubble bubble = GetComponent<SpeechBubble>();
                if (bubble != null)
				{
                    string text = DialogueManager.instance.playerYells[Random.Range(0, DialogueManager.instance.playerYells.Length)];
                    bubble.OpenSpeechBubble(text, playerYellDuration);
                    pedRepulsor.enabled = true;
                    pedRepulsor.RemoveAfterSec(playerYellDuration);
                    timeToNextYell = Time.time + playerYellCooldown;
				}
                break;
            case PlayerAbility.GRAPPLE_HOOK:
                GrappleHookController.instance?.DeployHook();
                break;
			case PlayerAbility.ROCKET_BOOST:
                if (frozen || rocketBoosting || Time.time < timeToNextRocketBoost)
				{
                    break;
				}
                rocketBoosting = true;
                rocketBoostRemaining = rocketBoostDuration;
                rocketBoostCanvas.SetActive(true);
                flameParticles.Play();
                boostSlider.value = 1f;
                
                break;
        }
	}

    private void NearestItemCheck()
    {
        Collider2D[] results = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector3(transform.localScale.x * itemPickupX, transform.localScale.y * itemPickupY, 0), 0, itemLayerMask, -Mathf.Infinity, Mathf.Infinity);
        //OnDrawGizmos();
        //m_Started = true;
        float closestDistance = 1000000;
        float currentDistance = 0;
        closestItem = null;
        foreach (Collider2D col in results)
        {
            currentDistance = Vector3.Distance(this.transform.position, col.transform.position);
            if (col.gameObject.GetComponent<ItemPedestal>() != null) {
                col.gameObject.GetComponent<ItemPedestal>().isClosest = false;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestItem = col.gameObject;
                }
            }
        }
        if (closestItem != null)
        {
            closestItem.gameObject.GetComponent<ItemPedestal>().isClosest = true;
        }
    }

    private void PickUp(InputAction.CallbackContext context)
    {
        NearestItemCheck();
        if (closestItem != null)
        {
            closestItem.gameObject.GetComponent<ItemPedestal>().PickUp(holding);
        }

        //interactEvent?.Invoke(holding);
    }

    public void setHolding(GameObject to){
        this.holding = to;
        if (to.GetComponent<Animation>() != null)
        {
            to.GetComponent<Animation>().playAutomatically = false;
            to.GetComponent<Animation>().Stop();
        }
        to.transform.position = holdAnchor.position;
        to.transform.eulerAngles = new Vector3(0, 0, to.GetComponent<Item>().spriteHoldRotation);
        //to.transform.rotation = new Quaternion(0, 0, 45, 0); // What the fuck are you doing here jonas????
        DialogueManager.currentCurio = this.holding.GetComponent<Item>().name;
    }
    
    public Sprite GiveNPCItem()
    {
        if (holding != null)
        {
            return holding.GetComponent<Item>().sprite;
        }
        return null;
    }

    public IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(crashDisableTime);
        cantMove = false;
        moveVector = moveAction.ReadValue<Vector2>().normalized;
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    /*    void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x * itemPickupX, transform.localScale.y * itemPickupY, 0));
        }*/

    public bool IsHolding()
	{
        if (holding == null)
		{
            return false;
		}

        return holding.GetComponent<Item>()?.curioName.Length > 0;
	}

    public Transform RemoveItem()
	{
        if (holding == null)
		{
            return null;
		}

        Transform item = holding.transform;
        holding = null;
        return item;
	}

    public void SetAbility(PlayerAbility newAbility)
	{
        currentAbility = newAbility;
        DialogueManager.SetAbilityName(PlayerAbilityDisplayNames[newAbility]);
	}
}
