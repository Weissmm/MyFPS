using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public bool isLook;

    public GameObject missleBoss;
    public Transform misslePortA;
    public Transform misslePortB;

    private Vector3 m_lookVec;
    private BoxCollider m_boxCollider;
    private Vector3 m_jumpHitTarget;

    private void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider>();
        m_rigdbody = GetComponent<Rigidbody>();
        m_animator = GetComponentInChildren<Animator>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_health = GetComponent<Health>();

        m_navMeshAgent.isStopped = true;

        //StartCoroutine(Think());
    }

    private void Update()
    {
        if (m_health.isDead)
        {
            StopAllCoroutines();
            GameManager.Instance.RestartGame();
        }
        if (isLook)
        {
            float horizon = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            m_lookVec = new Vector3(horizon, 0, vertical) * 5f;
            transform.LookAt(target.position + m_lookVec);
        }
        else
        {
            m_navMeshAgent.SetDestination(m_jumpHitTarget);
        }
    }

    public void StartAttack()
    {
        StartCoroutine(Think());
    } 

    private IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = UnityEngine.Random.Range(0, 5);

        switch (ranAction)
        {
            case 0: case 1: case 2: case 3:
                StartCoroutine(MissleShot());
                break;
            case 4:
                StartCoroutine(JumpHit());
                break;
        }
    }

    private IEnumerator MissleShot()
    {
        m_animator.SetTrigger("doShot");

        yield return new WaitForSeconds(0.2f);

        GameObject instantMissleA = Instantiate(missleBoss, misslePortA.position, misslePortA.rotation);
        MissleBoss missleBossA = instantMissleA.GetComponent<MissleBoss>();
        missleBossA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissleB = Instantiate(missleBoss, misslePortB.position, misslePortB.rotation);
        MissleBoss missleBossB = instantMissleB.GetComponent<MissleBoss>();
        missleBossB.target = target;

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(Think());
    }

    private IEnumerator JumpHit()
    {
        m_jumpHitTarget = target.position + m_lookVec;

        isLook = false;
        m_navMeshAgent.isStopped = false;
        m_boxCollider.enabled = false;

        m_animator.SetTrigger("doJumpHit");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;

        m_navMeshAgent.isStopped = true;
        m_boxCollider.enabled = true;

        yield return new WaitForSeconds(2f);

        StartCoroutine(JumpHit());
    }
}
