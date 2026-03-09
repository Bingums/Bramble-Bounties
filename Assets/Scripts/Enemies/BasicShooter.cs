using System.Collections;
using UnityEngine;

public class BasicShooter : MonoBehaviour
{
    //Movement Var
    public float moveSpeed = 2f;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;

    //Shooting Var
    float firingDistance;
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
        target = GameObject.Find("player").transform;
    
    }

    // Update is called once per frame
    void Update()
    {
        if(target){
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction; 
        }
        float distance = Vector2.Distance(transform.position, target.position);
        if(distance < firingDistance){
           
        }
       ShootGun(); 

 
    }

    
    private void ShootGun(){
        bulletTime -= Time.deltaTime;
        if(bulletTime > 0) return;
        bulletTime = timer;

        GameObject bulletObj = Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Destroy(bulletObj, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Weapon") || collision.CompareTag("EnemyProjectile"))
        {
            shooterRenderer.color = damageColor;
            StartCoroutine(Wait(0.5f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Weapon") || collision.CompareTag("EnemyProjectile"))
        {
           shooterRenderer.color = shooterColor;
        }
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
