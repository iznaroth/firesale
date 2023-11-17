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
        if (state == EGrappleState.RETRACTED && Time.time < timeToNextGrapple)
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
        // only start retracting if we were shooting or if we previously were holding something
        if (!(state == EGrappleState.SHOOTING || state == EGrappleState.RETRACTING_HIT))
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

        ped.Freeze();
        GameManager.Player.GetComponent<PlayerController>().Freeze();
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
        float retractDist = shotSpeed * Time.deltaTime;
        float length = Vector2.Distance(grappleHead.transform.position, grappleRoot.position);

        Vector3 retractDir = grappleHead.transform.position - grappleRoot.position;
        retractDir.Normalize();
        GameManager.Player.transform.position += retractDist * retractDir;
        UpdateView();

        if (length - retractDist < retractInteractDist)
		{
            DialogueManager.instance.StartDialogueInteraction(pedHit.gameObject);
            DialogueManager.DialogueInteractionEnded += HandleDialogueEnded;
		}
    }

    void HandleDialogueEnded()
	{
        RetractHookEmpty(true);

        DialogueManager.DialogueInteractionEnded -= HandleDialogueEnded;
    }
}
