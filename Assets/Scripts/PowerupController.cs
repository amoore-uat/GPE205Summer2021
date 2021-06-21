using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour
{
    public List<Powerup> m_activePowerups = new List<Powerup>();

    private void Update()
    {
        List<Powerup> expiredPowerups = new List<Powerup>();
        foreach (Powerup powerup in m_activePowerups)
        {
            powerup.duration -= Time.deltaTime;
            if (powerup.duration <= 0)
            {
                expiredPowerups.Add(powerup);
            }
        }
        foreach (Powerup expiredPowerup in expiredPowerups)
        {
            RemovePowerup(expiredPowerup);
        }
    }

    public void RemovePowerup(Powerup expiredPowerup)
    {
        expiredPowerup.OnDeactivate(this.gameObject);
        if (m_activePowerups.Contains(expiredPowerup))
        {
            m_activePowerups.Remove(expiredPowerup);
        }
    }

    public void AddPowerup(Powerup powerup)
    {
        if (!powerup.isPermanent)
        {
            m_activePowerups.Add(powerup);
        }
        powerup.OnActivate(this.gameObject);
    }
}
