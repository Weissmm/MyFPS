using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandard : MonoBehaviour
{
    public float maxLifeTime=5f;
    public float speed = 300f;

    public Transform root;
    public Transform tip;
    public float radius = 0.01f;
    public LayerMask hittableLayers = -1;

    public float damage = 10f;

    public GameObject impactVFX;
    public float impactVFXLifeTime = 5f;
    public float impactVFXSpawnOffset = 0.1f;

    public float trajectoryCorrectionDistance = 5f;

    private ProjectileBase m_projectileBase;
    private Vector3 m_velocity;
    private Vector3 m_lastRootPosition;

    private bool m_hasTrajectoryCorrected;
    private Vector3 m_correctionVector;

    private Vector3 m_consumedCorrectionVector;

    private void OnEnable()
    {
        m_projectileBase = GetComponent<ProjectileBase>();
        m_projectileBase.onShoot += OnShoot;
        Destroy(gameObject, maxLifeTime);
    }

    private void OnShoot()
    {
        m_lastRootPosition = root.position;
        m_velocity += transform.forward * speed;

        PlayerWeaponManager playerWeaponManager = m_projectileBase.owner.GetComponent<PlayerWeaponManager>();

        if (playerWeaponManager)
        {
            m_hasTrajectoryCorrected = false;

            Transform weaponCameraTransform = playerWeaponManager.weaponCamera.transform;

            Vector3 cameraToMuzzle = m_projectileBase.initialPositon - weaponCameraTransform.position;

            //Debug.DrawRay(weaponCameraTransform.position, cameraToMuzzle, Color.yellow, 6f);

            m_correctionVector = Vector3.ProjectOnPlane(-cameraToMuzzle, weaponCameraTransform.forward);

            //Debug.DrawRay(weaponCameraTransform.position, -cameraToMuzzle, Color.red,6f);
            //Debug.DrawRay(weaponCameraTransform.position, weaponCameraTransform.forward, Color.green, 6f);
            //Debug.DrawRay(weaponCameraTransform.position, m_correctionVector, Color.magenta, 6f);
        }
    }

    private void Update()
    {
        transform.position += m_velocity * Time.deltaTime;

        transform.forward = m_velocity.normalized;

        if (!m_hasTrajectoryCorrected && m_consumedCorrectionVector.sqrMagnitude < m_correctionVector.sqrMagnitude)
        {
            Vector3 correctionLeft = m_correctionVector - m_consumedCorrectionVector;

            float distanceThisFrame = (root.position - m_lastRootPosition).magnitude;

            Vector3 correctionThisFrame = (distanceThisFrame / trajectoryCorrectionDistance) * m_correctionVector;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
            m_consumedCorrectionVector += correctionThisFrame;

            if (Mathf.Abs(m_consumedCorrectionVector.sqrMagnitude - m_correctionVector.sqrMagnitude) < Mathf.Epsilon)
            {
                m_hasTrajectoryCorrected = true;
            }

            transform.position += correctionThisFrame;

        }

        RaycastHit cloestHit = new RaycastHit();
        cloestHit.distance = Mathf.Infinity;
        bool foundHit = false;

        Vector3 displaceMentSinceLastFrame=tip.position-m_lastRootPosition;

        RaycastHit[] hits = Physics.SphereCastAll(m_lastRootPosition, 
            radius, 
            displaceMentSinceLastFrame.normalized,
            displaceMentSinceLastFrame.magnitude, 
            hittableLayers, 
            QueryTriggerInteraction.Collide);

        foreach(RaycastHit hit in hits)
        {
            if (IsHitValid(hit) && hit.distance < cloestHit.distance)
            {
                cloestHit = hit;
                foundHit = true;
            }
        }

        if (foundHit)
        {
            if (cloestHit.distance <= 0)
            {
                cloestHit.point = root.position;
                cloestHit.normal = -transform.forward;
            }

            OnHit(cloestHit.point,cloestHit.normal,cloestHit.collider);
        }
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.isTrigger)
        {
            return false;
        }
        return true;
    }

    private void OnHit(Vector3 point,Vector3 normal,Collider collider)
    {
        Damageable damageable = collider.GetComponent<Damageable>();

        if (damageable)
        {
            damageable.InflictDamage(damage);
        }

        if (impactVFX != null)
        {
            GameObject impactVFXInstance = Instantiate(impactVFX, 
                point + normal * impactVFXSpawnOffset, 
                Quaternion.LookRotation(normal));

            if (impactVFXLifeTime > 0)
            {
                Destroy(impactVFXInstance, impactVFXLifeTime);
            }
        }

        Destroy(gameObject);
    }
}
