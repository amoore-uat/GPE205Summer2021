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
}
