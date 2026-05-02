using Combat;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public int damage;
    public float killBullet;
    
    public void InitializeEnemyBullet(int attack, float bulletSpeed, float range, Vector2 direction)
    {
        speed = bulletSpeed;
        damage = attack;
        killBullet = range / bulletSpeed;
        
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * speed;
        
        Destroy(gameObject, killBullet);
    }
    
    private void OnTriggerEnter2D(Collider2D collision){
        // bullet disappears if hit with melee or your bullets
        if(collision.CompareTag("Melee") || collision.CompareTag("Weapon") || collision.CompareTag("Terrain")) 
        {
            Destroy(gameObject);
        } // damages player and enemies (bullets spawns in shooter, need to change)
        else if(collision.CompareTag("Player")) //|| collision.CompareTag("Enemy"))
        {
            

            collision.GetComponentInParent<playerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
