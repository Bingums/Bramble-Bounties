using UnityEngine;

public class BarBottle : MonoBehaviour
{
    private GameObject player;
    public GameObject burningPuddle;
    public GameObject puddle;

    private Rigidbody2D rb;
    public float force = 3;
    private Vector3 direction;
    private float time = 3;

    private bool flag = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    
        direction = player.transform.position - transform.position;
        
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        
        time -= Time.deltaTime;

        if(time < 0){
            rb.linearVelocity = new Vector2(0, 0);
            if(!flag){
            puddle.SetActive(true);
            flag = true;
            }
        }

        if(time < -10){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        //player.GetComponent<Health>.health -= 2;
        if(collision.tag == "Player"){
            rb.linearVelocity = new Vector2(0, 0);
            puddle.SetActive(true);
            time = -1;
            flag = true;

        }else if(collision.tag == "EnemyProjectile"){
            puddle.SetActive(false);
            burningPuddle.SetActive(true);
        }
    }
}
