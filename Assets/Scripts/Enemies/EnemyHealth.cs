using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    void takeDamage(int damage){
        health -= damage;
        if(health <= 0){
            //To add a animation call a new method and call destroy there
            Destroy(gameObject);
        }
    }
    

}
