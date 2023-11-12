using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PedestrianAI : MonoBehaviour
{
    public Collider2D viewCone;
    public Transform rotationRoot;
    public float maxSpeed = 2f;
    public float startSpeed = 1f;
    public int minMiddleNodes = 0;
    public int maxmiddleNodes = 3;
    public float pedRadius = 1f;
    public LayerMask viewMask;

    [Header("Force Settings")]
    public float goalForceStrength = 5f;
    public float avoidanceForceStrength = 5f;
    public float avoidanceBonusRadius = 0.5f;
    public float minCollisionTime = 0.1f;
    public float turnSpeed = 1f;

    public UnityAction OnRemove;

    Rigidbody2D rb;
    List<PathingNode> path = new List<PathingNode>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = transform.right * startSpeed;

        if (PedestrianManager.instance)
        {
            PedestrianManager.instance.RegisterPedestrian(this);
        }

        GeneratePath();
    }

    void GeneratePath()
    {
        int middleNodes = Random.Range(minMiddleNodes, maxmiddleNodes + 1);

        middleNodes = Mathf.Min(middleNodes, PathingNode.middleNodes.Count);
        List<PathingNode> availableNodes = new List<PathingNode>(PathingNode.middleNodes);

        for (int i = 0; i < middleNodes; i++)
        {
            int nextIndex = Random.Range(0, availableNodes.Count);
            path.Add(availableNodes[nextIndex]);
            availableNodes.RemoveAt(nextIndex);
        }

        availableNodes = new List<PathingNode>(PathingNode.exitNodes);

        while (availableNodes.Count > 0)
        {
            int index = Random.Range(0, availableNodes.Count);
            if (Vector2.Distance(availableNodes[index].transform.position, transform.position) < 1f)
            {
                availableNodes.RemoveAt(index);
                continue;
            }

            path.Add(availableNodes[index]);
            break;
        }

        if (path.Count < 1)
        {
            Debug.LogError("Error: " + name + " generated a path with length 0, removing it from game");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (path.Count < 1)
        {
            CompletePath();
            return;
        }

        UpdatePath();

        if (path.Count < 1)
        {
            return;
        }

        CalculateAndApplyForces();

        UpdateView();
    }

    void UpdatePath()
    {
        PathingNode goalNode = path[0];
        float distToGoal = Vector2.Distance(goalNode.transform.position, transform.position);

        if (distToGoal < pedRadius + goalNode.nodeRadius)
        {
            path.RemoveAt(0);
            if (path.Count < 1)
            {
                CompletePath();
                return;
            }
        }
    }

    void CalculateAndApplyForces()
    {
        Vector2 force = Vector2.zero;

        // first add goal force;
        Vector2 goalDir = path[0].transform.position - transform.position;
        goalDir.Normalize();
        force += goalForceStrength * goalDir;

        // now do avoidance forces
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = viewMask;
        List<Collider2D> overlaps = new List<Collider2D>();

        Physics2D.OverlapCollider(viewCone, filter, overlaps);

        foreach (Collider2D collider in overlaps)
        {
            if (collider.gameObject == this) continue;

            PedestrianAI otherPed;
            if (otherPed = collider.GetComponent<PedestrianAI>())
            {
                Vector2 relPos = otherPed.transform.position - transform.position;
                Vector2 relVel = otherPed.GetComponent<Rigidbody2D>().velocity - rb.velocity;
                float totalRad = otherPed.pedRadius + pedRadius + avoidanceBonusRadius;

                float ttc = TimeToCollision(relPos, relVel, totalRad);

                if (ttc < 0f)
                {
                    // no collision, let's move on
                    continue;
                }

                Vector2 collisionPoint = relPos + relVel * ttc;
                Vector2 normal = collisionPoint.normalized; // normallized vector that points TO the collision

                force += avoidanceForceStrength * (-normal) / (ttc);
            }
            else
            {
                // then this is a normal obstacle
                Vector2 closestPoint = collider.ClosestPoint(transform.position);
                Vector2 dir = closestPoint - (Vector2)transform.position;
                Vector2 normal = dir.normalized;
                float componentVelocity = Vector2.Dot(normal, rb.velocity);
                float dist = dir.magnitude - pedRadius - avoidanceBonusRadius;
                float ttc = dist / componentVelocity;

                if (dist < 0) // already in collision
                {
                    ttc = minCollisionTime;
                }

                if (ttc < 0f) continue; // no collision

                force += avoidanceBonusRadius * (-normal) / (ttc);
            }
        }

        rb.AddForce(force);
        Vector2 newVel = rb.velocity;
        newVel = Vector2.ClampMagnitude(newVel, maxSpeed);
        rb.velocity = newVel;
    }

    float TimeToCollision(Vector2 pos, Vector2 vel, float rad)
    {
        float sqrSpeed = vel.sqrMagnitude;
        float sqrDist = pos.sqrMagnitude;
        float posDotVel = Vector2.Dot(pos, vel);

        float sqrDisc = posDotVel * posDotVel - sqrSpeed * (sqrDist - rad * rad);
        if (sqrDisc < 0f)
        {
            return -1f;
        }

        float disc = Mathf.Sqrt(sqrDisc);

        float t1 = (-posDotVel - disc) / sqrSpeed;
        if (t1 >= 0f)
        {
            return Mathf.Max(t1, minCollisionTime);
        }

        float t2 = (-posDotVel + disc) / sqrSpeed;
        if (t2 >= 0f)
        {
            // it means that we're in a collision already
            return minCollisionTime;
        }

        return -1f;
    }

    void CompletePath()
    {
        OnRemove.Invoke();
        Destroy(gameObject);
    }

    void UpdateView()
    {
        rotationRoot.right = Vector2.Lerp(rotationRoot.right, rb.velocity.normalized, Time.deltaTime * turnSpeed).normalized;
    }
}
