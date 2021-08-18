using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class TankKilledEvent : UnityEvent<GameObject> { }

[RequireComponent(typeof(TankData))]
public class Health : MonoBehaviour, IAttackable
{
    [SerializeField] public TankKilledEvent OnTankKilled;
    private TankData m_data;
    private float m_currentHealth;
    public AudioClip deathSoundEffect;

    private void Start()
    {
        OnTankKilled.AddListener(GameManager.Instance.HandleTankKilled);
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
            if (m_currentHealth > m_data.maxHealth)
            {
                m_currentHealth = m_data.maxHealth;
            }
        }
    }

    public void ApplyDamage(Attack attackData)
    {
        // Die if health hits zero.
        CurrentHealth -= attackData.m_damage;
        if (CurrentHealth <= 0f)
        {
            Die(attackData);
        }
        
    }

    public void OnAttacked(Attack attackData)
    {
        // TODO: Extract this debug information into a separate component.
        // TODO: Add a guardian clause in case the attacker is a null game object.
        Debug.Log(this.gameObject.name + " was attacked by " + attackData.m_attacker.name + " for " + attackData.m_damage.ToString() + " damage.");
        // TODO: Replace this with a method that receives the attack data.
        ApplyDamage(attackData);
    }

    public void Die(Attack attackData)
    {
        if (deathSoundEffect != null)
        {
            float soundEffectVolume = GameManager.Instance.masterVolume * GameManager.Instance.sfxVolume;
            AudioSource.PlayClipAtPoint(deathSoundEffect, transform.position, soundEffectVolume);
        }
        // Do the death stuff.
        // Award points to the tank that killed this tank if they should earn points.
        for (int i=0; i<=GameManager.Instance.Players.Count-1; i++)
        {
            Debug.Log("Checking " + attackData.m_attacker.name + " against " + GameManager.Instance.Players[i].name);
            if (attackData.m_attacker == GameManager.Instance.Players[i])
            {
                GameManager.Instance.PlayerScores[i].score += m_data.points;
                Debug.Log(attackData.m_attacker.name + " earned " + m_data.points + " points.");
            }
        }
        // Tell Game Manager that a tank died.
        
        Debug.Log("Tank was killed");
        HUD tankHUD = GetComponent<HUD>();
        if (tankHUD != null)
        {
            tankHUD.HandleGameOver(this.gameObject);
        }
        OnTankKilled.Invoke(this.gameObject);
        // If this player ran out of lives, detatch the camera

        Destroy(this.gameObject);
    }
}
