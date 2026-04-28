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
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name } defeated");
            Defeat();
        }
    }

    public void Defeat()
    {
        Destroy(gameObject);
    }
    
}
