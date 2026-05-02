using UnityEngine;

public class BartenderBullet : MonoBehaviour
{
   private Rigidbody2D rb;
    public float force;
    public float killBullet;
    private Vector3 direction;
    private GameObject Bartender;
    private Bartender bartender;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bartender = GameObject.Find("Bartender");
        bartender = Bartender.GetComponent<Bartender>();
        direction = bartender.getLocation();
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, killBullet);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }
    
    private void OnTriggerEnter2D(Collider2D collision){
        // bullet disappears if hit with melee or your bullets
        if(collision.CompareTag("Weapon") || collision.CompareTag("PlayerProjectile")) 
        {
            Destroy(gameObject);
        } // damages player and enemies (bullets spawns in shooter, need to change)
        else if(collision.CompareTag("Player")) //|| collision.CompareTag("Enemy"))
        {
            
            collision.GetComponent<playerHealth>().playerDamage(6);
            Destroy(gameObject);
        }
        else if(collision.CompareTag("Puddle")){
            Destroy(gameObject);
        }
    }
    
}
