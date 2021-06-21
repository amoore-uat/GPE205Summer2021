using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public List<GameObject> pickupPrefabs = new List<GameObject>();
    public GameObject spawnedPickup;
    private float respawnTimer = 3f;
    public float timeToRespawn = 3f;
    // Start is called before the first frame update
    void Start()
    {
        SpawnRandomPickup();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedPickup == null)
        {
            respawnTimer -= Time.deltaTime;
        }
        if (respawnTimer <= 0)
        {
            SpawnRandomPickup();
        }

    }

    private void SpawnRandomPickup()
    {
        // TODO: Update this later to pick a prefab at random.
        spawnedPickup = Instantiate(pickupPrefabs[0], transform.position, Quaternion.identity);
        respawnTimer = timeToRespawn;
    }
}
