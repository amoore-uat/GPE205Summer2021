using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
public class TankShooter : MonoBehaviour
{
    private TankData m_data;
    public GameObject m_cannonBallPrefab;
    public float m_cooldownTimer = 0f;
    public GameObject m_firePoint;

    private void Start()
    {
        m_data = gameObject.GetComponent<TankData>();
    }

    private void Update()
    {
        if (!CanShoot())
        {
            m_cooldownTimer -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        if (CanShoot())
        {
            Debug.Log("Pew Pew");
            // Instantiate a cannon ball.
            GameObject cannonBall = Instantiate(m_cannonBallPrefab, m_firePoint.transform.position, Quaternion.identity);
            // Add force to the cannon ball.
            cannonBall.GetComponent<Rigidbody>().AddForce(transform.forward * m_data.cannonBallForce, ForceMode.Impulse);
            // Get a reference to the cannon ball class
            CannonBall m_cannonBallData = cannonBall.GetComponent<CannonBall>();
            // Set the attacker
            m_cannonBallData.m_attackData.m_attacker = this.gameObject;
            // Set the damage
            m_cannonBallData.m_attackData.m_damage = m_data.damagePerShot;
            // Reset the cooldown timer
            m_cooldownTimer = m_data.secondsPerShot;
        }
    }

    public bool CanShoot()
    {
        return (m_cooldownTimer <= 0f);
    }
}
