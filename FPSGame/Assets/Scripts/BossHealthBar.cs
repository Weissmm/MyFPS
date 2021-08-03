using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image bossHealthBarImage;

    private Health m_health;

    private void Start()
    {
        Boss boss = FindObjectOfType<Boss>();
        if (boss)
        {
            m_health = boss.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (m_health)
        {
            bossHealthBarImage.fillAmount = m_health.currentHealth / m_health.maxHealth;
        }
    }
}
