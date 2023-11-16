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

    List<Vector2> ropeVertices;
    LineRenderer lineRenderer;
    float segmentLength;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ropeVertices = new List<Vector2>(segments);
        segmentLength = totalLength / segments;

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

        for (int i = 0; i < segments; i++)
		{
            Vector2 oldPos = ropeVertices[i];
            Vector2 toVertex = oldPos - prevPos;
            toVertex = segmentLength * toVertex.normalized;

            Vector2 newPos = prevPos + toVertex;
            ropeVertices[i] = newPos;

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
