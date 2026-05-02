using UnityEngine;

public class Boss : EnemyController
{
    public AudioClip gunfireSFX;
    public AudioClip SlamSFX;

    private AudioSource audioSource;
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
    protected override void Start()
    {
        base.Start();
        target = EnemySpawnManager.Instance.player;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        scoreValue = 10000;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
                    Debug.Log("CHARGE");
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
        moveDirection = (target.position - transform.position).normalized;
        Debug.Log("LOG");
        GameObject shotBullet = Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation);
        shotBullet.GetComponent<EnemyBullet>().InitializeEnemyBullet(attack, bulletSpeed, range, moveDirection);
        bulletCount --;
        audioSource.PlayOneShot(gunfireSFX);
    }

    void OnTriggerEnter2D(Collider2D collision){
            
        GameObject player = GameObject.Find("player");
        if(collision.tag == "Player"){
            collision.GetComponent<playerHealth>().playerDamage(8);
            audioSource.PlayOneShot(SlamSFX);
        }
    }


}
