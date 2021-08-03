using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weaponRoot;

    public Transform weaponMuzzle;

    public GameObject muzzleFlashPrefab;

    public ProjectileBase projectilePrefab;

    public Vector3 muzzleWorldVelocity { get; private set; }

    public float delayBetweenShots = 0.1f;

    private float m_lastShotTime = Mathf.NegativeInfinity;

    public bool isWeaponActive { get; private set; }
    public GameObject owner { get; set; }
    public GameObject sourcePrefab { get; set; }

    public void ShowWeapon(bool show)
    {
        weaponRoot.SetActive(show);
        isWeaponActive = show;
    }

    public bool HandleShootInputs(bool inputHeld)
    {
        if (inputHeld)
        {
            return TryShoot();
        }

        return false;
    }

    private bool TryShoot()
    {
        if (m_lastShotTime + delayBetweenShots < Time.time)
        {
            HandleShoot();
            print("SHot");
            return true;
        }

        return false;
    }

    private void HandleShoot()
    {
        if (projectilePrefab != null)
        {
            Vector3 shotDirection = weaponMuzzle.forward;
            ProjectileBase newProjiectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                weaponMuzzle.rotation, weaponMuzzle.transform);

            newProjiectile.Shoot(this);
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                weaponMuzzle.rotation, weaponMuzzle.transform);

            Destroy(muzzleFlashInstance, 2);
        }
        m_lastShotTime = Time.time;
    }


}
