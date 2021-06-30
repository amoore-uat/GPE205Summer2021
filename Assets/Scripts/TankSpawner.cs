using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    public GameObject m_spawnedTank;
    public bool HasTank
    {
        get
        {
            return (m_spawnedTank != null);
        }
    }

    private void Awake()
    {
        GameManager.Instance.tankSpawnPoints.Add(this);
    }
    /// <summary>
    /// Spawns in a tank at this spawn point.
    /// </summary>
    /// <param name="tank"></param>
    public GameObject SpawnTank(GameObject tank)
    {
        m_spawnedTank = Instantiate(tank, this.transform.position, Quaternion.identity);
        return m_spawnedTank;
    }
}
