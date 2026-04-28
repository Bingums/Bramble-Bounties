using Combat;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    public float force;
    public float killBullet;
    private Vector3 direction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = EnemySpawnManager.EnemySpawnManagerInstance.player;
    
        direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        
        Destroy(gameObject, killBullet);
    }
    
    private void OnTriggerEnter2D(Collider2D collision){
        // bullet disappears if hit with melee or your bullets
        if(collision.CompareTag("Weapon")) 
        {
            Destroy(gameObject);
        } // damages player and enemies (bullets spawns in shooter, need to change)
        else if(collision.CompareTag("Player")) //|| collision.CompareTag("Enemy"))
        {
            collision.GetComponent<playerController>().TakeDamage(4);
            Destroy(gameObject);
        }
    }
}
