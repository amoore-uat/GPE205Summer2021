using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float m_cannonballLifespan = 2f;
    public Attack m_attackData = new Attack();

    // Update is called once per frame
    void Update()
    {
        m_cannonballLifespan -= Time.deltaTime;

        if (m_cannonballLifespan <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If we run into something that can take damage, then apply that damage.
        IAttackable[] attackables = collision.gameObject.GetComponents<IAttackable>();
        if (attackables == null)
        {
            return;
        }
        foreach (IAttackable attackable in attackables)
        {
            attackable.OnAttacked(m_attackData);
        }
        // Destroy the cannnonball no matter what it ran into.
        Destroy(this.gameObject);
    }
}
