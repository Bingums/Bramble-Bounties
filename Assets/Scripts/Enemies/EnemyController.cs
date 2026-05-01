using UnityEngine;
using Combat;

public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] protected int baseMaxHealth;
    [SerializeField] protected int baseAttack;
    [SerializeField] protected float baseMoveSpeed;
    
    protected int maxHealth;
    protected int currentHealth;
    protected int attack;
    protected float moveSpeed;
    
    protected float distance;
    protected Rigidbody2D rb;
    protected Transform target;
    protected Vector2 moveDirection;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        attack = baseAttack;
        moveSpeed = baseMoveSpeed;
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
    
    //protected virtual void Attack() { }
    
    
    public void ScaleStats(float healthMultiplier, float attackMultiplier, float moveSpeedMultiplier)
    {
        maxHealth = Mathf.RoundToInt(baseMaxHealth * healthMultiplier);
        currentHealth = maxHealth;
        attack = Mathf.RoundToInt(baseAttack * attackMultiplier);
        moveSpeed = baseMoveSpeed * moveSpeedMultiplier;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        Room currentRoom = EnemySpawnManager.Instance.GetCurrentRoom();
        if (currentRoom != null)
        {
            currentRoom.DecreaseEnemyCount();
        }
        
        Destroy(gameObject);
    }
    
}
