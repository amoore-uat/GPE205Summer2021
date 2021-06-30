using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int m_mapSeed;
    public int m_rows = 3;
    public int m_columns = 3;
    public enum RandomSeedType { RANDOM, MAPOFTHEDAY, SEEDED};
    public RandomSeedType m_seedType;
    private float m_roomWidth = 50.0f;
    private float m_roomHeight = 50.0f;
    private Room[,] m_rooms;

    public GameObject enemyTankPrefab;

    public GameObject[] gridPrefabs;

    private void Start()
    {
        // Clear out the grid - "which column" is our X, "which row" is our Y
        m_rooms = new Room[m_columns, m_rows];
        GenerateGrid();
        SpawnPlayerTanks(2);
        SpawnEnemyTanks();
    }

    public GameObject RandomRoomPrefab()
    {
        if (gridPrefabs.Length >= 1)
        {
            return gridPrefabs[UnityEngine.Random.Range(0, gridPrefabs.Length)];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Finds an available spawn point and spawns in a tank at that point.
    /// </summary>
    /// <param name="TankToSpawn"></param>
    public GameObject SpawnTank(GameObject TankToSpawn)
    {
        List<TankSpawner> availableSpawners = new List<TankSpawner>();
        foreach (TankSpawner spawnPoint in GameManager.Instance.tankSpawnPoints)
        {
            if (!spawnPoint.HasTank)
            {
                availableSpawners.Add(spawnPoint);
            }
        }
        TankSpawner randomSpawnPoint = GetRandomSpawnPoint(availableSpawners);
        GameObject spawnedTank = randomSpawnPoint.SpawnTank(TankToSpawn);
        return spawnedTank;
    }

    public void SpawnEnemyTanks()
    {
        // We need to spawn at least one of each personality.
        // Spawn the aggressive personality.
        GameObject enemyTank = SpawnTank(enemyTankPrefab);
        enemyTank.GetComponent<AIController>().personality = AIPersonality.Aggressive;
        enemyTank.gameObject.name = "Aggressive Enemy Tank";
    }

    public void SpawnPlayerTanks(int numberOfTanks)
    {
        for (int i=0;i<numberOfTanks;i++)
        {
            GameObject spawnedPlayer = SpawnTank(GameManager.Instance.playerPrefab);
            GameManager.Instance.Players[i] = spawnedPlayer;
            spawnedPlayer.gameObject.name = "Player " + (i + 1);
        }
    }

    private TankSpawner GetRandomSpawnPoint(List<TankSpawner> availableSpawners)
    {
        return availableSpawners[UnityEngine.Random.Range(0, availableSpawners.Count)];
    }

    public int DateToInt(DateTime dateToUse)
    {
        // Add our date up and return it
        return dateToUse.Year + dateToUse.Month + dateToUse.Day + dateToUse.Hour + dateToUse.Minute + dateToUse.Second + dateToUse.Millisecond;
    }


    public void GenerateGrid()
    {
        // Seed the random number generator.
        switch (m_seedType)
        {
            case RandomSeedType.MAPOFTHEDAY:
                m_mapSeed = DateToInt(DateTime.Today);
                UnityEngine.Random.InitState(m_mapSeed);
                break;
            case RandomSeedType.RANDOM:
                m_mapSeed = DateToInt(DateTime.Now);
                UnityEngine.Random.InitState(m_mapSeed);
                break;
            case RandomSeedType.SEEDED:
                UnityEngine.Random.InitState(m_mapSeed);
                break;
            default:
                Debug.Log("[MapGenerator - Generate Grid] Unknown seed type");
                break;

        }
        for (int row = 0; row < m_rows; row++)
        {
            for (int column = 0; column < m_columns; column++)
            {
                float xPosition = m_roomWidth * column;
                float zPosition = m_roomHeight * row;
                Vector3 newPosition = new Vector3(xPosition, 0.0f, zPosition);

                // Create a new grid at the appropriate location
                GameObject tempRoomObj = Instantiate(RandomRoomPrefab(), newPosition, Quaternion.identity) as GameObject;

                // Set its parent
                tempRoomObj.transform.parent = this.transform;

                // Give it a meaningful name
                tempRoomObj.name = "Room_" + column + "," + row;

                // Assign the room to the rooms array
                m_rooms[column, row] = tempRoomObj.GetComponent<Room>();

                if (row == 0)
                {
                    m_rooms[column, row].doorSouth.SetActive(true);
                }
                if (row == (m_rows-1))
                {
                    m_rooms[column, row].doorNorth.SetActive(true);
                }
                if (column == 0)
                {
                    m_rooms[column, row].doorWest.SetActive(true);
                }
                if (column == (m_columns - 1))
                {
                    m_rooms[column, row].doorEast.SetActive(true);
                }
            }
        }
    }

}
