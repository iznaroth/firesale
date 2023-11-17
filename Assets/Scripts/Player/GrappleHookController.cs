using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EGrappleState
{
    RETRACTED,
    SHOOTING,
    RETRACTING_EMPTY,
    RETRACTING_HIT,
    FROZEN,
}

[RequireComponent(typeof(LineRenderer))]
public class GrappleHookController : MonoBehaviour
{
    public static GrappleHookController instance;

    public float shotSpeed = 10f;
    public float maxDist = 5f;
    public float retractSpeed = 10f;
    public float grappleCooldown = 2f;
    public float startOffset = 0.25f;
    public float retractEndDist = 0.25f;
    public float retractInteractDist = 0.25f;
    public GrappleHookHead grappleHead;
    public Transform grappleRoot;

    public UnityAction<PedestrianAI> OnGrappleHit;

    LineRenderer lineRenderer;
    EGrappleState state;
    float timeToNextGrapple;
    Vector2 shotDir;
    bool ignoreHits = false;
    PedestrianAI pedHit;

	private void Awake()
	{
        instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        state = EGrappleState.RETRACTED;
        OnGrappleHit += HandlePedestrianHit;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
		{
            case EGrappleState.SHOOTING:
                UpdateShooting();
                break;
            case EGrappleState.RETRACTING_EMPTY:
                UpdateRetractingEmpty();
                break;
            case EGrappleState.RETRACTING_HIT:
                UpdateRetractingHit();
                break;
        }
    }

    public void DeployHook()
	{
        if (state != EGrappleState.RETRACTED || Time.time < timeToNextGrapple)
		{
            return;
		}

        state = EGrappleState.SHOOTING;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shotDir = mousePos - (Vector2)grappleRoot.position;
        shotDir.Normalize();

        Vector2 startPos = (Vector2)grappleRoot.position + shotDir * startOffset;
        grappleHead.gameObject.SetActive(true);
        grappleHead.transform.position = startPos;
        grappleHead.transform.up = shotDir;

        lineRenderer.positionCount = 2;

        UpdateView();
    }

    // immediately disable hook
    public void CancelHook()
	{
        lineRenderer.positionCount = 0;
        grappleHead.gameObject.SetActive(false);
        timeToNextGrapple = Time.time + grappleCooldown;
        ignoreHits = false;

        state = EGrappleState.RETRACTED;
	}

    // start retracting the hook
    public void RetractHookEmpty(bool setIgnorePeds = false)
	{
        // ignore if we're already retracted
        if (state == EGrappleState.RETRACTED)
		{
            return;
		}

        state = EGrappleState.RETRACTING_EMPTY;
        ignoreHits = setIgnorePeds;
	}

    public void HandlePedestrianHit(PedestrianAI ped)
	{
        if (ignoreHits || !(state == EGrappleState.SHOOTING || state == EGrappleState.RETRACTING_EMPTY))
		{
            return;
		}

        state = EGrappleState.RETRACTING_HIT;
        pedHit = ped;

        ped.Freeze(false);
        GameManager.Player.GetComponent<PlayerController>().Freeze();
        GameManager.Player.GetComponent<StartInteraction>().TargetSpecificPed(ped.gameObject);

        MakeThemScream scream = ped.GetComponent<MakeThemScream>();
        if (scream != null)
		{
            scream.Scream();
		}
	}

    void UpdateView()
	{
        if (state == EGrappleState.RETRACTED)
		{
            return;
		}

        Vector3 ropeDir = grappleHead.transform.position - grappleRoot.position;

        grappleHead.transform.up = ropeDir.normalized;

        Vector3[] ropePoints = { grappleRoot.position, grappleHead.transform.position };
        lineRenderer.SetPositions(ropePoints);

	}

    void UpdateShooting()
	{
        grappleHead.transform.position += (Vector3)shotDir * shotSpeed * Time.deltaTime;
        UpdateView();

        float length = Vector2.Distance(grappleHead.transform.position, grappleRoot.position);
        if (length > maxDist)
		{
            RetractHookEmpty();
		}
	}

    void UpdateRetractingEmpty()
	{
        float retractDist = shotSpeed * Time.deltaTime;
        float length = Vector2.Distance(grappleHead.transform.position, grappleRoot.position);

        if (length - retractDist < retractEndDist)
		{
            CancelHook();
            return;
		}

        Vector3 returnDir = grappleRoot.position - grappleHead.transform.position;
        returnDir.Normalize();
        grappleHead.transform.position += retractDist * returnDir;
        UpdateView();
	}

    void UpdateRetractingHit()
	{
        float retractDist = retractSpeed * Time.deltaTime;
        float length = Vector2.Distance(grappleHead.transform.position, grappleRoot.position);

        Vector3 retractDir = grappleHead.transform.position - grappleRoot.position;
        retractDir.Normalize();
        GameManager.Player.transform.position += retractDist * retractDir;
        UpdateView();

        if (length - retractDist < retractInteractDist)
		{
            if (GameManager.Player.GetComponent<PlayerController>().IsHolding())
			{
                pedHit.Freeze();
                DialogueManager.instance.StartDialogueInteraction(pedHit.gameObject, Start_Conditions.Grappled);
                DialogueManager.DialogueInteractionEnded += HandleDialogueEnded;
                state = EGrappleState.FROZEN;
            }
            else
			{
                pedHit.UnFreeze();
                GameManager.Player.GetComponent<PlayerController>().UnFreeze();
                GameManager.Player.GetComponent<Rigidbody2D>().velocity = retractDir * retractSpeed;
                RetractHookEmpty(true);
            }
		}
    }

    void HandleDialogueEnded()
	{
        RetractHookEmpty(true);

        DialogueManager.DialogueInteractionEnded -= HandleDialogueEnded;
    }
}
