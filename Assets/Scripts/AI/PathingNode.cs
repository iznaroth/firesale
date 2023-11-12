using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingNode : MonoBehaviour
{
    public static List<PathingNode> allNodes = new List<PathingNode>();
    public static List<PathingNode> entryNodes = new List<PathingNode>();
    public static List<PathingNode> exitNodes = new List<PathingNode>();
    public static List<PathingNode> middleNodes = new List<PathingNode>();

    public bool isEntry;
    public bool isExit;
    public float nodeRadius = 1f;
    public bool debugDraw = false;

    private void Awake()
    {
        if (!allNodes.Contains(this))
        {
            allNodes.Add(this);
        }

        if (isEntry && !entryNodes.Contains(this))
        {
            entryNodes.Add(this);
        }

        if (isExit && !exitNodes.Contains(this))
        {
            exitNodes.Add(this);
        }

        if (!isEntry && !isExit && !middleNodes.Contains(this))
        {
            middleNodes.Add(this);
        }
    }

    private void OnDestroy()
    {
        allNodes.Remove(this);

        if (isEntry)
        {
            entryNodes.Remove(this);
        }

        if (isExit)
        {
            exitNodes.Remove(this);
        }

        if (!isEntry && !isExit)
        {
            middleNodes.Remove(this);
        }
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, nodeRadius);

        if (isEntry)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * 2f);
        }
    }
}
