using UnityEngine;
using Combat;

public class EnemyData : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attack;
    

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
        Room currentRoom = EnemySpawnManager.EnemySpawnManagerInstance.GetCurrentRoom();
        if(currentRoom != null)
            currentRoom.DecreaseEnemyCount();
        Destroy(gameObject);
    }
    
}
