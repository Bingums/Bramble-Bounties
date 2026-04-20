using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Combat;

public class playerController : MonoBehaviour, IDamageable
{
    private const int KnifeSlot = 0;
    private const int GunSlot = 1;
    private static readonly Vector3 GunHoldOffset = new Vector3(0.35f, 0.05f, 0f);

    [SerializeField] private float BASE_SPEED = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float maxStamina = 1000f;
    [SerializeField] private float staminaDrainRate = 2f;     // per second
    [SerializeField] private float staminaRegenRate = 1f;     // per second
    [SerializeField] private float dashMultiplier = 2f;
    [SerializeField] private float dashCooldownTime = 3f;
    [SerializeField] private Image staminaFill;
    [SerializeField] private float currentStamina = 3f;

    [Header("Combat Debug")] 
    [SerializeField] private WeaponData sword;

    [SerializeField] private WeaponData rifle;
    [SerializeField] private WeaponData shotgun;
    [SerializeField] private WeaponData lever;
    
    private bool isOnCooldown = false;

    private Rigidbody2D rb;

    private Animator animator;
    
    private List<IInteractable> interactables = new List<IInteractable>();

    public WeaponData[] weapons = new WeaponData[2];
    public int curSlot = 0;

    [SerializeField] private float maxHealth;
    [SerializeField] private float curHealth;

    public GameObject displayedWeapon;
    private GameObject equippedWeaponObject;
    private int equippedSlot = -1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        EquipWeapon(KnifeSlot);
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

        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            EquipWeapon(KnifeSlot);
        } else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            EquipWeapon(GunSlot);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpgradeWeapon(0, sword);
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpgradeWeapon(1, rifle);
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UpgradeWeapon(1, lever);
        } else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UpgradeWeapon(1, shotgun);
        }

        if(interactables != null && Input.GetKeyDown(KeyCode.E)) {
            GetClosestInteractable().Interact();
        }
    }

    private void EquipWeapon(int slot)
    {
        if (slot == equippedSlot)
        {
            return;
        }

        if (slot < 0 || slot >= weapons.Length || weapons[slot] == null || weapons[slot].weaponPrefab == null)
        {
            Debug.LogWarning($"Cannot equip weapon slot {slot}. Check the player weapons array.");
            return;
        }

        if (displayedWeapon == null)
        {
            Debug.LogWarning("Cannot display equipped weapon because displayedWeapon is not assigned.");
            curSlot = slot;
            equippedSlot = slot;
            return;
        }

        if (equippedWeaponObject != null)
        {
            Destroy(equippedWeaponObject);
        }

        curSlot = slot;
        equippedSlot = slot;

        Transform weaponParent = weapons[slot].isMelee ? transform : displayedWeapon.transform;
        equippedWeaponObject = Instantiate(weapons[slot].weaponPrefab, weaponParent);
        equippedWeaponObject.name = weapons[slot].weaponPrefab.name;
        equippedWeaponObject.transform.localPosition = weapons[slot].isMelee ? Vector3.zero : GunHoldOffset;
        equippedWeaponObject.transform.localRotation = Quaternion.Euler(weapons[slot].rotation);
        MatchWeaponSorting(equippedWeaponObject);

        if (equippedWeaponObject.name.Contains("word"))
        {
            equippedWeaponObject.name = "Knife";
        }

        animator.Rebind();
        animator.Update(0f);
    }

    private void MatchWeaponSorting(GameObject weaponObject)
    {
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer weaponRenderer = weaponObject.GetComponent<SpriteRenderer>();

        if (playerRenderer == null || weaponRenderer == null)
        {
            return;
        }

        weaponRenderer.sortingLayerID = playerRenderer.sortingLayerID;
        weaponRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
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
        curHealth -= damage;
    }

    public void UpgradeWeapon(int slot, WeaponData toUpgrade)
    {
        weapons[slot] = toUpgrade;
        equippedSlot = -1;
        EquipWeapon(slot);
    }
}
