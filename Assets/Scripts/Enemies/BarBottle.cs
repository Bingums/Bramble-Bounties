using UnityEngine;

public class BarBottle : MonoBehaviour
{
    private GameObject player;
    public GameObject burningPuddle;
    public GameObject puddle;
    public AudioClip bottleBreakingSFX;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    public float force = 3;
    private Vector3 direction;
    private float time = 3;//Until bottle breaks;
    private float lifeSpan = 15;
    public int bountyScaler;

    private bool flag = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    
        direction = player.transform.position - transform.position;
        
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        
        time -= Time.deltaTime;
        lifeSpan -= Time.deltaTime;

        if(time < 0){
            rb.linearVelocity = new Vector2(0, 0);
            if(!flag){
                puddle.SetActive(true);
                flag = true;
            }
        }

        if(lifeSpan < 0){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(!flag){
            collision.GetComponent<playerHealth>().playerDamage(6);
        }
        
        if(collision.tag == "Player"){
            rb.linearVelocity = new Vector2(0, 0);
            audioSource.PlayOneShot(bottleBreakingSFX);
            puddle.SetActive(true);
            time = -1;
            flag = true;

        }else if(collision.tag == "BarProjectile"){
            puddle.SetActive(false);
            burningPuddle.SetActive(true);
        }
    }
}
