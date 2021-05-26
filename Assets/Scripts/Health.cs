using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankData))]
public class Health : MonoBehaviour, IAttackable
{
    private TankData m_data;
    private float m_currentHealth;

    private void Start()
    {
        m_data = gameObject.GetComponent<TankData>();

        if (m_data != null)
        {
            m_currentHealth = m_data.maxHealth;
        }
    }
    public float CurrentHealth
    {
        get
        {
            return m_currentHealth;
        }
        set
        {
            m_currentHealth = value;
            if (m_currentHealth <= 0f)
            {
                // Die if health hits zero.
                Die();
            }
            if (m_currentHealth > m_data.maxHealth)
            {
                m_currentHealth = m_data.maxHealth;
            }
        }
    }

    public void OnAttacked(Attack attackData)
    {
        // TODO: Extract this debug information into a separate component.
        // TODO: Add a guardian clause in case the attacker is a null game object.
        Debug.Log(this.gameObject.name + " was attacked by " + attackData.m_attacker.name + " for " + attackData.m_damage.ToString() + " damage.");
        CurrentHealth -= attackData.m_damage;
    }

    public void Die()
    {
        // Do the death stuff.
        // TODO: Award points to the tank that killed this tank if they should earn points.
        Debug.Log("Tank was killed");
        Destroy(this.gameObject);
    }
}
