using UnityEngine;
using Combat;

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
    protected Transform target;
    protected Vector2 moveDirection;
    private bool isDefeated;

    protected int scoreValue = 10;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        attack = baseAttack;
        moveSpeed = baseMoveSpeed;
        bulletSpeed = baseBulletSpeed;
        range = baseRange;
    }
    
    protected virtual void Start()
    {
        target = EnemySpawnManager.Instance.player;
    }
    
    protected virtual void Update()
    {
        if(target){
            distance = Vector2.Distance(transform.position, target.position);
            moveDirection = (target.position - transform.position).normalized;
        }
    }
    
    protected virtual void FixedUpdate()
    {
        if(target)
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
    }
    
    public void ScaleStats(float healthMultiplier, float attackMultiplier, float moveSpeedMultiplier)
    {
        maxHealth = Mathf.RoundToInt(baseMaxHealth * healthMultiplier);
        currentHealth = maxHealth;
        attack = Mathf.RoundToInt(baseAttack * attackMultiplier);
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
    }

    public void TakeDamage(int damage)
    {
        if (isDefeated)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{name} took {damage} damage. HP: {currentHealth}");
        if (currentHealth <= 0)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        if (isDefeated)
        {
            return;
        }

        isDefeated = true;

        Room currentRoom = EnemySpawnManager.Instance != null ? EnemySpawnManager.Instance.GetCurrentRoom() : null;
        if (currentRoom != null)
        {
            currentRoom.DecreaseEnemyCount();
        }
        
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
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
        Destroy(gameObject);
    }
    
}
