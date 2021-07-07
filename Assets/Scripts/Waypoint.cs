using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.allWaypoints.Add(this.gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.allWaypoints.Remove(this.gameObject);
    }
}
