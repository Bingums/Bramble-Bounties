using System.Collections;
using UnityEngine;
using Combat;
using UnityEngine.Diagnostics;

public class BasicShooter : MonoBehaviour
{
    public EnemyData shooterData;

    //Movement Var
    private float moveSpeed = 2f;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;

    //Shooting Var
    public float firingDistance;
    private float timer = 1;
    private float bulletTime;
    public GameObject bullet;
    public Transform spawnPoint;

    SpriteRenderer shooterRenderer;
    Color shooterColor;
    Color damageColor = new Color(0.85f, 0.24f, 0.24f);

    private void Awake(){
         rb = GetComponent<Rigidbody2D>();

         shooterRenderer = GetComponent<SpriteRenderer>();
         shooterColor = shooterRenderer.color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target){
            moveDirection = (target.position - transform.position).normalized;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        if(distance < firingDistance) {
           moveSpeed = 0;
           ShootGun();
        } else if(distance > firingDistance) {
            moveSpeed = 2;
        }
        

 
    }
    private void FixedUpdate(){
        if(target){
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    
    private void ShootGun(){
        bulletTime -= Time.deltaTime;
        if(bulletTime > 0) return;
        bulletTime = timer;

        Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Weapon")) //|| collision.CompareTag("EnemyProjectile"))
        {
            if(collision.CompareTag("Player"))
            {
                collision.GetComponent<playerController>().TakeDamage(5);
            }
            
            shooterRenderer.color = damageColor;
            StartCoroutine(Wait(0.5f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        shooterRenderer.color = shooterColor;
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
