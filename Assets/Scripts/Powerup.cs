using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Powerup
{
    public float healthModifier;
    public float speedModifier;
    public float fireSpeedModifier;

    public float duration;
    public bool isPermanent;

    public void OnActivate(GameObject target)
    {
        TankData targetData = target.GetComponent<TankData>();
        Health targetHealth = target.GetComponent<Health>();
        if (targetData != null)
        {
            targetData.moveSpeed += speedModifier;
            targetData.secondsPerShot -= fireSpeedModifier;
        }
        if (targetHealth != null)
        {
            targetHealth.CurrentHealth += healthModifier;
        }
    }

    public void OnDeactivate(GameObject target)
    {
        TankData targetData = target.GetComponent<TankData>();
        Health targetHealth = target.GetComponent<Health>();
        if (targetData != null)
        {
            targetData.moveSpeed -= speedModifier;
            targetData.secondsPerShot += fireSpeedModifier;
        }
        if (targetHealth != null)
        {
            targetHealth.CurrentHealth -= healthModifier;
        }
    }
}
