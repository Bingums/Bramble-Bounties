using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private int damage;
    public float killBullet;

    public void InitializePlayerBullet(WeaponData data, Vector2 direction)
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
        if(collision.CompareTag("Terrain"))
        {
            Destroy(gameObject);
        } else if(collision.CompareTag("Enemy") || collision.CompareTag("Bartender"))
        {
            collision.GetComponentInParent<EnemyController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
