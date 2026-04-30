using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private int damage;
    public float killBullet;
    private bool hitSomething = false;

    public void InitializeBullet(WeaponData data, Vector2 direction)
    {
        speed = data.shotSpeed;
        damage = data.damage;
        killBullet = data.range / data.shotSpeed;
        
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * speed;
        
        Destroy(gameObject, killBullet);
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitSomething) return;
        
        if(collision.CompareTag("Terrain") || collision.CompareTag("NPC"))
        {
            hitSomething = true;
            Destroy(gameObject);
        } else if(collision.CompareTag("Enemy") || collision.CompareTag("Bartender"))
        {
            hitSomething = true;
            collision.GetComponent<EnemyController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
