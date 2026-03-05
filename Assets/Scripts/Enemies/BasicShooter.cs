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

    private void Awake(){
         rb = GetComponent<Rigidbody2D>();
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

    
    void ShootGun(){
        bulletTime -= Time.deltaTime;
        if(bulletTime > 0) return;
        bulletTime = timer;

        GameObject bulletObj = Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
    
    }
}
