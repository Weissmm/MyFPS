using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public bool isInstanceDie;
    public float maxHealth = 100f;
    public bool isDead { get; private set; }


    
    private float m_curentHealth;
    private MeshRenderer[] m_meshes;
    private Animator m_animator;

    public float currentHealth
    {
        get => m_curentHealth;
        set => m_curentHealth = value;
    }

    private void Awake()
    {
        m_meshes = GetComponentsInChildren<MeshRenderer>();
        m_animator = GetComponentInChildren<Animator>();
        m_curentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        m_curentHealth -= damage;
        m_curentHealth = Mathf.Clamp(m_curentHealth, 0, maxHealth);

        Debug.Log(m_curentHealth);

        StartCoroutine(OnDamage());
    }

    IEnumerator OnDamage()
    {
        foreach(MeshRenderer mesh in m_meshes)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        if (m_curentHealth > 0)
        {
            foreach(MeshRenderer mesh in m_meshes)
            {
                mesh.material.color = Color.white;
            }
        }else if (!isDead)
        {
            foreach(MeshRenderer mesh in m_meshes)
            {
                mesh.material.color = Color.gray;
            }

            isDead = true;

            if (isInstanceDie)
            {
                Destroy(gameObject);
            }
            else
            {
                m_animator.SetTrigger("doDie");
                Destroy(gameObject, 3f);
            }

        }
    }
}
