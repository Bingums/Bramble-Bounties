using Combat;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force;
    public float killBullet;
    private Vector3 direction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    
        direction = player.transform.position - transform.position;
        
        Destroy(gameObject, killBullet);
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }

    private void OnTriggerEnter2D(Collider2D collision){
        // bullet disappears if hit with melee or your bullets
        if(collision.CompareTag("Melee") || collision.CompareTag("PlayerProjectile")) 
        {
            Destroy(gameObject);
        } // damages player and enemies
        else if(collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IDamageable>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
