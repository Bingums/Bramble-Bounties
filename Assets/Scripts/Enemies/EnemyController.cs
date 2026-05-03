using System;
using System.Collections;
using UnityEngine;
using Combat;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] protected int baseMaxHealth;
    [SerializeField] protected int baseAttack;
    [SerializeField] protected float baseMoveSpeed;
    [SerializeField] protected float baseBulletSpeed;
    [SerializeField] protected float baseRange;
     //[SerializeField] protected int baseScoreValue;

    protected int maxHealth;
    protected int currentHealth;
    protected int attack;
    protected float moveSpeed;
    protected float bulletSpeed;
    protected float range;
    //protected int scoreValue;
    
    protected float distance;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform target;
    protected Vector2 moveDirection;
    protected bool isDefeated;

    public int scoreValue = 10;
    public int enemyType;
    
    protected HUDController hc;

    public event Action<EnemyController> OnHealthInitialized;
    public event Action<EnemyController, int, int> OnHealthChanged;
    public event Action<EnemyController> OnDefeated;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;
        animator  = GetComponent<Animator>();
        
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        attack = baseAttack;
        moveSpeed = baseMoveSpeed;
        bulletSpeed = baseBulletSpeed;
        range = baseRange;

        OnHealthInitialized?.Invoke(this);
        OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
    }
    
    protected virtual void Start()
    {
        StartCoroutine(WaitForHUD());
        target = EnemySpawnManager.Instance.player;
    }
    
    protected virtual void Update()
    {
        if (isDefeated)
            return;
        
        if(enemyType == 1){
            distance = Vector2.Distance(transform.position, target.position);
            moveDirection = (target.position - transform.position).normalized;
        }
        else if(target){
            distance = Vector2.Distance(transform.position, target.position);
            moveDirection = (target.position - transform.position).normalized;
        }
        
        animator.SetFloat("TargetX", moveDirection.x);
        animator.SetBool("isMoving", rb.linearVelocity.magnitude > 0.1f);
    }
    
    protected virtual void FixedUpdate()
    {
        if (isDefeated)
            return;
        
        Debug.Log("START - Velocity: " + (moveDirection * moveSpeed) + " | MoveDir: " + moveDirection + " | Speed: " + moveSpeed);
        
        if(enemyType == 1){
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
        else if (target)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
        
        Debug.Log("END - Velocity: " + (moveDirection * moveSpeed) + " | MoveDir: " + moveDirection + " | Speed: " + moveSpeed);
    }
    
    public void ScaleStats(float healthMultiplier, float attackMultiplier, float moveSpeedMultiplier)
    {
        maxHealth = Mathf.RoundToInt(baseMaxHealth * healthMultiplier);
        currentHealth = maxHealth;
        attack = Mathf.RoundToInt(baseAttack * attackMultiplier);
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
        
        OnHealthChanged?.Invoke(this,  currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDefeated)
        {
            return;
        }

        currentHealth -= damage;
        OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
        //Debug.Log($"{name} took {damage} damage. HP: {currentHealth}");
        if (currentHealth <= 0)
        {
            if(this is Boss)
            {
                Defeat();
                hc.Victory();
            }
            else
            {
                Defeat();
            }
        }
    }

    private void Defeat()
    {
        if (isDefeated)
        {
            return;
        }

        isDefeated = true;
        OnDefeated?.Invoke(this);

        Room currentRoom = EnemySpawnManager.Instance != null ? EnemySpawnManager.Instance.GetCurrentRoom() : null;
        if (currentRoom != null)
        {
            currentRoom.DecreaseEnemyCount();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (EnemySpawnManager.Instance != null)
        {
            float ammoSpawn = Random.Range(0f, 1f);
            if(ammoSpawn <= 0.1f && EnemySpawnManager.Instance.ammoPickup != null)
                Instantiate(EnemySpawnManager.Instance.ammoPickup, transform.position + new Vector3(0.15f, 0, 0), Quaternion.identity);
            float healthSpawn = Random.Range(0f, 1f);
            if (healthSpawn <= 0.1f && EnemySpawnManager.Instance.healthPickup != null)
                Instantiate(EnemySpawnManager.Instance.healthPickup, transform.position + new Vector3(0.15f, 0, 0), Quaternion.identity);
        }
        animator.SetTrigger("defeated");
        moveSpeed = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.linearDamping = 10000f;
        Destroy(gameObject, 1f);
    }
    
    IEnumerator WaitForHUD()
    {
        while (hc == null)
        {
            hc = GameObject.Find("HUD Container").GetComponentInChildren<HUDController>();
            yield return null;
        }
    }
}
