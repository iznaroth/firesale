using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidPoint : MonoBehaviour
{
    public float avoidStrength = 1f;
    public float pedSpeedMultiplier = 2f;
    public float pedSpeedInfluenceDistance = 10f;

    private void OnEnable()
    {
        PedestrianManager.AddAvoidancePoint(this);
    }

    private void OnDisable()
    {
        PedestrianManager.RemoveAvoidancePoint(this);
    }
}
