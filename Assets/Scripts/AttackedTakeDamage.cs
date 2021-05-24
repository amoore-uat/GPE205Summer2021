using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedTakeDamage : MonoBehaviour, IAttackable
{
    public float currentHP = 10f;
    public float maxHP = 10f;
    public void OnAttacked(Attack attackData)
    {
        currentHP -= attackData.m_damage;
    }
}
