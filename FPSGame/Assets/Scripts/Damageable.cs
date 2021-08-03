using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private Health m_health;

    private void Awake()
    {
        m_health = GetComponent<Health>();
    }

    public void InflictDamage(float damage)
    {
        m_health.TakeDamage(damage);
    }
}
