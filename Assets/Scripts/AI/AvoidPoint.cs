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

    public void ReduceAfterSec(float sec)
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        disableCoroutine = StartCoroutine(Reduce(sec));
    }

    IEnumerator Disable(float delay)
	{
        yield return new WaitForSeconds(delay);

        this.enabled = false;
	}
    IEnumerator Reduce(float delay)
    {
        avoidStrength = 50;
        pedSpeedMultiplier = 4f;
        pedSpeedInfluenceDistance = 15;

    yield return new WaitForSeconds(delay);

        avoidStrength = 5;
        pedSpeedMultiplier = 1.5f;
        pedSpeedInfluenceDistance = 3.5f;
    }
}
