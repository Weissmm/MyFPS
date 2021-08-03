using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController instance;

    public Camera playerCamera;
    public float gravityDownForce=20f;
    public float maxSpeedOnGround = 8f;
    public float moveSharpnessOnGround = 15f;
    public float rotationSpeed = 200f;
    public float maxHealth = 200f;

    public float cameraHeightRatio = 0.9f;

    private CharacterController m_characterController;
    private PlayerInputHandler m_playerInputHandler;
    private float m_targetCharacterHeight = 1.8f;
    private float m_cameraVerticalAngle = 0f;
    private float m_currentHealth;
    private bool m_isBossAttack;

    public float currentHealth => m_currentHealth;

    public Vector3 characterVelocity { get; set; }

    private void Awake()
    {
        instance = this;
        m_currentHealth = maxHealth;
    }

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerInputHandler = GetComponent<PlayerInputHandler>();

        m_characterController.enableOverlapRecovery = true;

        UpdateCharacterHeight();
    }

    private void Update()
    {
        HandleCharacterMovement();
    }

    private void UpdateCharacterHeight()
    {
        m_characterController.height = m_targetCharacterHeight;
        m_characterController.center = Vector3.up * m_characterController.height * 0.5f;

        playerCamera.transform.localPosition = Vector3.up * m_characterController.height * 0.9f;
    }

    private void HandleCharacterMovement()
    {
        transform.Rotate(new Vector3(0, m_playerInputHandler.GetMouseLookHorizontal() * rotationSpeed, 0), Space.Self);

        m_cameraVerticalAngle+= m_playerInputHandler.GetMouseLookVertical() * rotationSpeed;

        m_cameraVerticalAngle = Mathf.Clamp(m_cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(m_cameraVerticalAngle, 0, 0);

        Vector3 worldSpaceMoveInput = transform.TransformVector(m_playerInputHandler.GetMoveInput());

        if (m_characterController.isGrounded)
        {
            Vector3 targetVelocity = worldSpaceMoveInput * maxSpeedOnGround;

            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity,
                moveSharpnessOnGround * Time.deltaTime);
        }
        else
        {
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        if (m_isBossAttack)
        {
            characterVelocity += transform.forward * -3;
        }

        m_characterController.Move(characterVelocity * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHitPlayer(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHitPlayer(other);
    }

    private void OnHitPlayer(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            m_currentHealth -= enemyBullet.damage;

            StartCoroutine(OnDamage());

            if (other.GetComponent<Rigidbody>())
            {
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Melee Area"))
        {
            MeleeAttacker meleeAttacker = other.GetComponent<MeleeAttacker>();

            m_currentHealth -= meleeAttacker.damage;

            m_isBossAttack = other.name == "Boss Melee Area";

            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        print(m_currentHealth);
        if (m_currentHealth < 0)
        {
            OnDie();
        }

        yield return new WaitForSeconds(0.2f);
        m_isBossAttack = false;
    }

    private void OnDie()
    {
        SceneManager.LoadScene("Level1");
    }

}
