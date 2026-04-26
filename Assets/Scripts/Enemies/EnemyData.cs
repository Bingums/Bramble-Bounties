using UnityEngine;
using Combat;

public class EnemyData : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attack;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Defeat();
        }
    }

    public void Defeat()
    {
        Room currentRoom = EnemySpawnManager.Instance.GetCurrentRoom();
        if(currentRoom != null)
            currentRoom.DecreaseEnemyCount();
        Destroy(gameObject);
    }
    
}
