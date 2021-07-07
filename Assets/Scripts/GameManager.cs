using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] Players = new GameObject[2];
    public List<TankSpawner> tankSpawnPoints = new List<TankSpawner>();
    public GameObject playerPrefab;
    public int numberOfPlayers = 1;
    public List<GameObject> allWaypoints = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject GetRandomWaypoint()
    {
        if (allWaypoints.Count < 1)
        {
            Debug.LogWarning("There are no waypoints");
            return null;
        }
        else
        {
            return allWaypoints[Random.Range(0, allWaypoints.Count)];
        }
    }
}
