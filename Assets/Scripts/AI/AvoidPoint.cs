using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidPoint : MonoBehaviour
{
    public float avoidStrength = 1f;
    public float pedSpeedMultiplier = 2f;
    public float pedSpeedInfluenceDistance = 10f;

    Coroutine disableCoroutine;

    private void OnEnable()
    {
        PedestrianManager.AddAvoidancePoint(this);
    }

    private void OnDisable()
    {
        PedestrianManager.RemoveAvoidancePoint(this);
    }

    public void RemoveAfterSec(float sec)
	{
        if (disableCoroutine != null)
		{
            StopCoroutine(disableCoroutine);
		}

        disableCoroutine = StartCoroutine(Disable(sec));
	}

    IEnumerator Disable(float delay)
	{
        yield return new WaitForSeconds(delay);

        this.enabled = false;
	}
}
