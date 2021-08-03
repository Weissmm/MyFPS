using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBarImage;

    private PlayerCharacterController m_characterController;

    private void Start()
    {
        m_characterController = FindObjectOfType<PlayerCharacterController>();
    }

    private void Update()
    {
        if (m_characterController)
        {
            healthBarImage.fillAmount = m_characterController.currentHealth / m_characterController.maxHealth;
        }
    }
}
