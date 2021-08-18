using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public  List<GameObject> Players = new List<GameObject>();
    public  int[] PlayerLives = new int[2];
    public ScoreData[] PlayerScores = new ScoreData[2];
    public  List<TankSpawner> tankSpawnPoints = new List<TankSpawner>();
    public GameObject playerPrefab;
    public int numberOfPlayers = 1;
    public List<GameObject> allWaypoints = new List<GameObject>();
    public GameObject enemyTankPrefab;
    public float masterVolume;
    public float sfxVolume = 1f;
    public int livesToStartWith = 3;
    public List<ScoreData> highScoreTable = new List<ScoreData>();

    // Components
    public AudioMixer mixer;


    public RandomSeedType m_seedType;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            // Load Options Data only if this is the correct game manager.
            LoadOptionsData();
        }
        PlayerScores[0] = new ScoreData();
        PlayerScores[1] = new ScoreData();
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

    public void SpawnEnemyTanks()
    {
        // We need to spawn at least one of each personality.
        // Spawn the aggressive personality.
        GameObject enemyTank = SpawnTank(enemyTankPrefab);
        enemyTank.GetComponent<AIController>().personality = AIPersonality.Aggressive;
        enemyTank.gameObject.name = "Aggressive Enemy Tank";
        // Change tank color
        enemyTank.GetComponent<AIController>().ChangeTankColors();
    }

    public GameObject SpawnTank(GameObject TankToSpawn)
    {
        List<TankSpawner> availableSpawners = new List<TankSpawner>();
        foreach (TankSpawner spawnPoint in tankSpawnPoints)
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

    public TankSpawner GetRandomSpawnPoint(List<TankSpawner> availableSpawners)
    {
        return availableSpawners[UnityEngine.Random.Range(0, availableSpawners.Count)];
    }

    public void SpawnPlayerTanks(int numberOfTanks)
    {
        for (int i = 0; i < numberOfTanks; i++)
        {
            GameObject spawnedPlayer = SpawnTank(playerPrefab);
            Players.Add(spawnedPlayer);
            spawnedPlayer.gameObject.name = "Player " + (i + 1);
            PlayerLives[i] = livesToStartWith;
        }
        if (numberOfTanks == 1)
        {
            // We need a reference to the camera to be able to adjust it.
            Camera camera = Players[0].GetComponentInChildren<Camera>();
            camera.rect = new Rect(0f, 0f, 1f, 1f);
        }
        else
        {
            // Adjust camera for player 1
            Camera camera = Players[0].GetComponentInChildren<Camera>();
            camera.rect = new Rect(0f, 0f, 0.5f, 1f);
            // Adjust camera for player 2
            camera = Players[1].GetComponentInChildren<Camera>();
            camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
            Players[1].GetComponent<PlayerController>().m_playerInputScheme = PlayerController.InputScheme.ArrowKeys;
        }
    }

    public void SaveOptionsData()
    {
        PlayerPrefs.SetInt("MAPSEEDTYPE", (int)m_seedType);
        // Save data after modifying the values
        PlayerPrefs.Save();
    }

    public void LoadOptionsData()
    {
        m_seedType = (RandomSeedType)PlayerPrefs.GetInt("MAPSEEDTYPE", 0);
        masterVolume = PlayerPrefs.GetFloat("MASTERVOLUME", 1f);
        ChangeMasterVolume(masterVolume);
        
    }

    private void OnApplicationQuit()
    {
        SaveOptionsData();
    }

    public void ChangeMasterVolume(float newVolume)
    {
        masterVolume = newVolume;
        mixer.SetFloat("MasterVolume", AdjustVolume(masterVolume));
    }

    public void HandleTankKilled(GameObject killedTank)
    {
        // Check to see if the tank was a player
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (killedTank == Players[i])
            {
                PlayerLives[i] -= 1;
                if (PlayerLives[i] > 0)
                {
                    // Respawn the player if they have lives
                    Players[i] = SpawnTank(playerPrefab);
                    Players[i].name = "Player " + i + 1;
                    // If multiplayer, adjust the camera
                    if (numberOfPlayers == 1)
                    {
                        // We need a reference to the camera to be able to adjust it.
                        Camera camera = Players[0].GetComponentInChildren<Camera>();
                        camera.rect = new Rect(0f, 0f, 1f, 1f);
                    }
                    else
                    {
                        // Adjust camera for player 1
                        Camera camera = Players[0].GetComponentInChildren<Camera>();
                        camera.rect = new Rect(0f, 0f, 0.5f, 1f);
                        // Adjust camera for player 2
                        camera = Players[1].GetComponentInChildren<Camera>();
                        camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                        Players[1].GetComponent<PlayerController>().m_playerInputScheme = PlayerController.InputScheme.ArrowKeys;
                    }

                    return;
                }
                else
                {
                    // Detach the camera.
                    Players[i].GetComponentInChildren<Camera>().transform.SetParent(GameManager.Instance.transform, true);
                    int sum = 0;
                    foreach (int lives in PlayerLives)
                    {
                        sum += lives;
                    }
                    if (sum < 1)
                    {
                        // Actual Game Over
                        // Update the high score table with each player's score.
                        for (int j=0; j < numberOfPlayers; j++)
                        {
                            UpdateHighScoreTable(PlayerScores[j]);
                        }
                        SceneManager.LoadScene("GameOver");
                    }
                    else
                    {
                        // Other player still going
                    }
                }
            }
        }
    }

    public float AdjustVolume(float volume)
    {
        return Mathf.Log10(volume) * 20f;
    }

    public void UpdateHighScoreTable(ScoreData scoreToAdd)
    {
        highScoreTable.Add(scoreToAdd);
        highScoreTable.Sort();
        highScoreTable.Reverse();
        highScoreTable = highScoreTable.GetRange(0, 5);
    }
}
