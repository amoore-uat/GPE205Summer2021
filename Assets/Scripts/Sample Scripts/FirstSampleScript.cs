using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSampleScript : MonoBehaviour
{
    private float m_currentHealth;
    public float m_maxHealth = 100f;
    
    private int m_lives;

    // Start is called before the first frame update
    void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        DoOneDamagePerSecond();
        LogCurrentHealth();
    }

    private void LogCurrentHealth()
    {
        Debug.Log(m_currentHealth);
    }

    /// <summary>
    /// Deals one damage to itself every second.
    /// </summary>
    private void DoOneDamagePerSecond()
    {
        m_currentHealth = m_currentHealth - 1f * Time.deltaTime;
    }

    // Not the best way to do this. Why would we need a parameter in this example?
    void PrintHealth(float currentHealth)
    {
        Debug.Log(m_currentHealth);
    }
}
