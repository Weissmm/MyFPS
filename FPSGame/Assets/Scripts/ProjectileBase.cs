using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileBase : MonoBehaviour
{
    public GameObject owner { get; private set; }
    public Vector3 initialPositon { get; private set; }
    public Vector3 initialDirection { get; private set; }
    public Vector3 inheritedMuzzleVelocity { get; private set; }

    public UnityAction onShoot;

    public void Shoot(WeaponController controller)
    {
        owner = controller.owner;
        initialPositon = transform.position;
        initialDirection = transform.forward;

        inheritedMuzzleVelocity = controller.muzzleWorldVelocity;

        if (onShoot != null)
        {
            onShoot.Invoke();
        }
    }
}
