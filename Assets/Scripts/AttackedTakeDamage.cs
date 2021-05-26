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

        // This is an example of referencing a variable in a game manager
        // GameManager.Instance.Player = this.gameObject;
        // This class is getting removed after we finish this milestone.
        // TODO: Nuke this class.
    }
}
