using UnityEngine;
public class Attack
{
    public GameObject m_attacker;
    public float m_damage;

    public Attack(GameObject m_attacker, float m_damage)
    {
        this.m_attacker = m_attacker;
        this.m_damage = m_damage;
    }

    public Attack()
    {
    }
}