using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.tankSpawnPoints.Add(this);
    }
    public void SpawnTank(GameObject tank)
    {
        Instantiate(tank, this.transform.position, Quaternion.identity);
    }
}
