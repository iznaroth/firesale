using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FloppySim : MonoBehaviour
{
    public Transform ropeRoot;
    public GameObject ropeVertexPrefab;
    public int segments = 5;
    public float totalLength = 2f;
    public Vector2 spawnDirection = new Vector2(-1f, 0f);
    public float maxBendDegrees = 15f;

    List<Vector2> ropeVertices;
    LineRenderer lineRenderer;
    float segmentLength;
    Vector2 startDir;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ropeVertices = new List<Vector2>(segments);
        segmentLength = totalLength / segments;
        startDir = spawnDirection.normalized;

        lineRenderer.positionCount = segments + 1;

        SpawnRope();
        UpdateLineRenderer();
    }

    void SpawnRope()
	{
        Vector2 pos = ropeRoot.position;

        for (int i = 0; i < segments; i++)
		{
            pos += spawnDirection.normalized * segmentLength;
            ropeVertices.Add(pos);
        }
    }

    void UpdateLineRenderer()
	{
        Vector3[] points = new Vector3[segments + 1];
        points[0] = ropeRoot.position;

        for (int i = 0; i < segments; i++)
		{
            points[i + 1] = ropeVertices[i];
		}

        lineRenderer.SetPositions(points);
	}

    void UpdateRope()
	{
        Vector2 prevPos = ropeRoot.position;
        Vector2 dir = startDir;

        for (int i = 0; i < segments; i++)
		{
            Vector2 oldPos = ropeVertices[i];
            Vector2 toVertex = oldPos - prevPos;
            toVertex = segmentLength * toVertex.normalized;

            float angle = Vector2.SignedAngle(dir, toVertex);

            if (Mathf.Abs(angle) > maxBendDegrees)
			{
                toVertex = Vector3.RotateTowards(dir, toVertex.normalized, maxBendDegrees * Mathf.Deg2Rad, 0f) * segmentLength;
			}

            Vector2 newPos = prevPos + toVertex;
            ropeVertices[i] = newPos;

            dir = (newPos - prevPos).normalized;
            prevPos = newPos;
		}
	}

    // Update is called once per frame
    void Update()
    {
        UpdateRope();
        UpdateLineRenderer();
    }
}
