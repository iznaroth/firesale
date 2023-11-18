using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianManager : MonoBehaviour
{
    public static PedestrianManager instance;

    static List<AvoidPoint> avoidancePoints = new List<AvoidPoint>();


    public List<GameObject> pedestrianPrefabs;
    public int maxPedestrians;
    public Vector2 spawnTimeBounds;

    List<PedestrianAI> currentPedestrians = new List<PedestrianAI>();
    float nextSpawnAttempt;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate Pedestrian manager found, destroying " + name);
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void RegisterPedestrian(PedestrianAI ped)
    {
        currentPedestrians.Add(ped);

        ped.OnRemove += () => { 
            currentPedestrians.Remove(ped);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawnAttempt)
        {
            TrySpawnPed();
            nextSpawnAttempt = Time.time + Random.Range(spawnTimeBounds.x, spawnTimeBounds.y);
        }
    }

    void TrySpawnPed()
    {
        if (currentPedestrians.Count >= maxPedestrians || pedestrianPrefabs.Count < 1)
        {
            return;
        }

        Vector2 spawnPoint = Vector2.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (PathingNode.entryNodes.Count > 0)
        {
            int index = Random.Range(0, PathingNode.entryNodes.Count);
            spawnPoint = PathingNode.entryNodes[index].transform.position;
            spawnRot = PathingNode.entryNodes[index].transform.rotation;
        }

        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Count)];

        Instantiate(prefab, spawnPoint, spawnRot);
    }

    public static void AddAvoidancePoint(AvoidPoint avoid)
    {
        if (!avoidancePoints.Contains(avoid))
        {
            avoidancePoints.Add(avoid);
        }
    }

    public static void RemoveAvoidancePoint(AvoidPoint avoid)
    {
        avoidancePoints.Remove(avoid);
    }

    public static List<AvoidPoint> GetAvoidancePoints()
    {
        return avoidancePoints;
    }
}
