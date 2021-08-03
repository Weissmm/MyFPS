using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MissleBoss : Bullet
{
    public Transform target;

    private NavMeshAgent m_navMeshAgent;


    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        m_navMeshAgent.SetDestination(target.position);
    }
}
