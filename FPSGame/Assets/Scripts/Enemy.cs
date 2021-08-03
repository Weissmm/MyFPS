using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { Melee,Range,Boss}

    public Type enemyType;

    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;

    protected Rigidbody m_rigdbody;
    protected NavMeshAgent m_navMeshAgent;
    protected Animator m_animator;
    protected Health m_health;


    private float m_targetRadius;
    private float m_targetRange;
    private bool m_isChase;
    private bool m_isAttack;

    private void Awake()
    {
        m_rigdbody = GetComponent<Rigidbody>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();
        m_health = GetComponent<Health>();

        //if (enemyType != Type.Boss)
        //{
        //    Invoke("ChaseStart", 2);
        //}
    }

    public void ChaseStart()
    {
        m_isChase = true;

        if (m_animator)
        {
            m_animator.SetBool("isWalk", true);
        }
    }

    private void FreeVelocity()
    {
        m_rigdbody.velocity = Vector3.zero;
        m_rigdbody.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (m_health.isDead)
        {
            StopAllCoroutines();
        }

        if (m_navMeshAgent.enabled&& !m_health.isDead && enemyType!=Type.Boss)
        {
            m_navMeshAgent.SetDestination(target.position);
            m_navMeshAgent.isStopped = !m_isChase;
        }
    }

    private void FixedUpdate()
    {
        if (m_isChase)
        {
            FreeVelocity();
        }

        if (enemyType != Type.Boss)
        {

            Targeting();
        }
    }

    private void Targeting()
    {
        if (enemyType == Type.Melee)
        {
            m_targetRadius = 0.5f;
            m_targetRange = 2f;
        }else if (enemyType == Type.Range)
        {
            m_targetRadius = 0.5f;
            m_targetRange = 10f;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_targetRadius, transform.forward
            ,m_targetRange,LayerMask.GetMask("Player"));

        if (hits.Length > 0 && !m_isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        m_isChase = false;
        m_isAttack = true;

        m_animator.SetBool("isAttack", true);

        if (enemyType == Type.Melee)
        {
            yield return new WaitForSeconds(0.2f);

            meleeArea.enabled = true;

            yield return new WaitForSeconds(1f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(1f);
        }
        else if (enemyType == Type.Range)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
            Rigidbody rigidbodyBullet = instantBullet.GetComponent<Rigidbody>();
            rigidbodyBullet.velocity = transform.forward * 20f;

            yield return new WaitForSeconds(2f);
        }

        m_isChase = true;
        m_isAttack = false;
        m_animator.SetBool("isAttack", false);
    }
}
