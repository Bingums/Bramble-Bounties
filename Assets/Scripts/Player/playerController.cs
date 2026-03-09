using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Combat;

public class playerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float BASE_SPEED = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float maxStamina = 3f;
    [SerializeField] private float staminaDrainRate = 2f;     // per second
    [SerializeField] private float staminaRegenRate = 1f;     // per second
    [SerializeField] private float dashMultiplier = 2f;
    [SerializeField] private float dashCooldownTime = 3f;
    [SerializeField] private Image staminaFill;
    private Rigidbody2D rb;

    [SerializeField] private float currentStamina = 3f;
    private bool isOnCooldown = false;

    private Animator animator;
    
    private List<IInteractable> interactables = new List<IInteractable>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 dir = new Vector2(horizontal, vertical).normalized;

        bool isDashing = Input.GetKey(KeyCode.LeftShift);

        float speedToUse = BASE_SPEED;

        // DASH LOGIC
        if (isDashing && currentStamina > 0 && !isOnCooldown)
        {
            speedToUse *= dashMultiplier;

            // Drain stamina over time
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                StartCoroutine(DashCooldown());
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

        //staminaFill.fillAmount = currentStamina / maxStamina;

        // Apply movement
        if (dir.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = dir * speedToUse;
        }
        // animator uses blend tree, only needs 2 inputs
        animator.SetFloat("InputX", horizontal);
        animator.SetFloat("InputY", vertical);

        if(interactables != null && Input.GetKeyDown(KeyCode.E)) {
            GetClosestInteractable().Interact();
        }
    }

    IEnumerator DashCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(dashCooldownTime);

        isOnCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable nearbyInteractable = collision.GetComponent<IInteractable>();
        interactables.Add(nearbyInteractable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable nearbyInteractable = collision.GetComponent<IInteractable>();
        interactables.Remove(nearbyInteractable);
    }

    private IInteractable GetClosestInteractable()
    {
        IInteractable closestInteractable = null;
        float closestDistance = 10000000000;

        foreach(IInteractable interactable in interactables)
        {
            float distance = Vector2.Distance(transform.position, interactable.transform.position);
            if(distance <= closestDistance) { 
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        return closestInteractable;
    }

    public void TakeDamage(int damage)
    {
        
    }
}
