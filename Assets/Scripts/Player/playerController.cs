using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    [SerializeField] private float BASE_SPEED = 5f;

    [Header("Sprint Settings")]
    [SerializeField] private float maxStamina = 3f;
    [SerializeField] private float staminaDrainRate = 2f;  
    [SerializeField] private float staminaRegenRate = 1f; 
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float sprintCooldownTime = 3f;
    [SerializeField] private Image staminaFill;
    private Rigidbody2D rb;

    [SerializeField] private float currentStamina;
    private bool isOnCooldown = false;

    [Header("Player Stats")]
    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(horizontal, vertical).normalized;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float speedToUse = BASE_SPEED;

        // DASH LOGIC
        if (isSprinting && currentStamina > 0 && !isOnCooldown)
        {
            speedToUse *= sprintMultiplier;

            // Drain stamina over time
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                StartCoroutine(SprintCooldown());
            }
        }
        else
        {
            // Regen stamina if not cooling down
            if (!isOnCooldown && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }

        staminaFill.fillAmount = currentStamina / maxStamina;

        // Apply movement
        if (dir.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = dir * speedToUse;
        }

        // Flip sprite
        if (horizontal > 0)
        {
            transform.rotation = new Quaternion(0, -1, 0, 0);
        }
        else if (horizontal < 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

    }

    IEnumerator SprintCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(sprintCooldownTime);

        isOnCooldown = false;
    }
}
