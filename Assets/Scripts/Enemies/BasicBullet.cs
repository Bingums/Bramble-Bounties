using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force;
    public float killBullet = 3;
    private Vector3 direction;

    private GameObject bartenderObj;
    private Bartender bartender;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    
        direction = player.transform.position - transform.position;

        
        bartenderObj = GameObject.FindGameObjectWithTag("Bartender");
        if(bartenderObj != null){
            BottleLocation();
        }

    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        killBullet -= Time.deltaTime;
        if(killBullet < 0){
            Destroy(gameObject);
        }
        /*Debug.Log((transform.position.x - direction.x) + " X Spot");
        Debug.Log((transform.position.y - direction.y) + " Y Shit");
        if((transform.position.x - direction.x) < .1f && (transform.position.y - direction.y) < .1f ){
            Destroy(gameObject);
            
        }*/
    }

    void OnTriggerEnter2D(Collider2D collision){
        //player.GetComponent<Health>.health -= 2;
        if(collision.tag == "Player"){
            Destroy(gameObject);
        }
         if(collision.tag == "Puddle"){
            Destroy(gameObject, .1f);
        }
    }

    void BottleLocation(){
        bartender = bartenderObj.GetComponent<Bartender>();
        direction = bartender.getLocation();
    }


}
