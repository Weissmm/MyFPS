using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.TriggerBasicEnemies();
        }
    }
}
