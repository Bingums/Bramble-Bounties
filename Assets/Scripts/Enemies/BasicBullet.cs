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
}
