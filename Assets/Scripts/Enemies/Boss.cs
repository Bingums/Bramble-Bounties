using UnityEngine;

public class Boss : MonoBehaviour
{

    public int moveSpeed = 2;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    private Vector2 playerLocation;

    private int randAttack = 0;
    private bool playerContactFlag = false;

    private float timer = 0;

    Vector2 chargeVect;

    //Shooting Var
    public float firingDistance;
    private float timerbullets = .25f;
    private float bulletTime;
    public GameObject bullet;
    public Transform spawnPoint;
    private int bulletCount = 10;

   // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = EnemySpawnManager.Instance.player;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        playerLocation = new Vector2(target.position.x, target.position.y);

        if(playerLocation.x < 25 && playerLocation.y < 25 && !playerContactFlag){
            playerContactFlag = true;
        }

        if(playerContactFlag){//Player has reached the boss and the boss will not stop pursuit

            if(randAttack == 0){
                randAttack = Random.Range(1,3);
                timer = 0;

            }else if(randAttack == 1){//Charge Attack
                timer -= Time.deltaTime;
                if(timer > -.35f){
                    moveSpeed = 2;
                    moveDirection = (target.position - transform.position).normalized;
                    rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
                }else if(timer > -2){
                    moveSpeed = 10;
                    rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed ;

                }else{
                    moveSpeed = 0;
                    rb.linearVelocity = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed ;
                    randAttack = -1;
                    timer = 0;

                }
            
            
            }else if(randAttack == 2){//Rapid  bullets
                if(bulletCount > 0){
                    ShootGun();
                }else{
                    bulletCount = 10;
                    randAttack = -1;
                }
                
            }else{//Boss moves towards the player like a sort of cooldown I guess idk
                moveSpeed = 2;
                moveDirection = (target.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed; 
                timer -= Time.deltaTime;
                if(timer < -3) randAttack = 0;
            }
        }
        
    }

    private void ShootGun(){
        bulletTime -= Time.deltaTime;
        if(bulletTime > 0) return;
        bulletTime = timerbullets;

        Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation);
        bulletCount --;
    }

    void OnTriggerEnter2D(Collider2D collision){
            
        GameObject player = GameObject.Find("player");
        if(collision.tag == "Player"){
            playerController other = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
            other.TakeDamage(2 * moveSpeed);
        }
    }


}
