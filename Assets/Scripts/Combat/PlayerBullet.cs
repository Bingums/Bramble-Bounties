using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float force;
    private int damage;
    public float killBullet;
    private Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (mousePos - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        
        Destroy(gameObject, killBullet);
    }

    public void InitializeBullet(WeaponData data)
    {
        force = data.shotSpeed;
        damage = data.damage;
        killBullet = data.range / data.shotSpeed;
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Terrain") || collision.CompareTag("NPC"))
        {
            Destroy(gameObject);
        } else if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyData>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
