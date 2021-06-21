using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Powerup m_powerup;

    private void OnTriggerEnter(Collider other)
    {
        PowerupController targetPowerupController = other.gameObject.GetComponent<PowerupController>();
        if (targetPowerupController != null)
        {
            targetPowerupController.AddPowerup(m_powerup);
            Destroy(this.gameObject);
        }
    }
}
