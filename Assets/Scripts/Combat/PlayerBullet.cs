using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float force;
    public float killBullet;
    private Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (mousePos - (Vector2)transform.position).normalized;
        
        Destroy(gameObject, killBullet);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EnemyProjectile"))
        {
            Destroy(gameObject);
        } else if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyData>().TakeDamage(6);
            Destroy(gameObject);
        }
    }
}
