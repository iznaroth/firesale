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
    public Vector2 downRootPos;
    public Vector2 downDirection;
    public int downSortOrder = 1;
    public Vector2 rightRootPos;
    public Vector2 rightDirection;
    public int rightSortOrder = 1;
    public Vector2 upRootPos;
    public Vector2 upDirection;
    public int upSortOrder = 1;
    public bool listenForPlayerDirection = true;

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

        if (listenForPlayerDirection)
		{
            GameManager.Player.GetComponent<PlayerController>().OnPlayerChangeDirection += HandleDirChange;

        }
    }

    public void HandleDirChange(FacingDir newDir)
	{
		switch (newDir)
		{
            case FacingDir.DOWN:
                FaceDown();
                break;
            case FacingDir.UP:
                FaceUp();
                break;
            case FacingDir.RIGHT:
                FaceRight();
                break;
            case FacingDir.LEFT:
                FaceLeft();
                break;
        }
	}

    public void FaceUp()
	{
        transform.localPosition = upRootPos;
        startDir = upDirection;
        lineRenderer.sortingOrder = upSortOrder;
	}

    public void FaceDown()
    {
        transform.localPosition = downRootPos;
        startDir = downDirection;
        lineRenderer.sortingOrder = downSortOrder;

    }

    public void FaceRight()
    {
        transform.localPosition = rightRootPos;
        startDir = rightDirection;
        lineRenderer.sortingOrder = rightSortOrder;

    }

    public void FaceLeft()
    {
        Vector2 pos = rightRootPos;
        pos.x *= -1f;

        Vector2 dir = rightDirection;
        dir.x *= -1f;

        transform.localPosition = pos;
        startDir = dir;
        lineRenderer.sortingOrder = rightSortOrder;

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
