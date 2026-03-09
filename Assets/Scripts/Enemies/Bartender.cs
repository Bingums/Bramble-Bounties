using UnityEngine;

public class Bartender : MonoBehaviour
{
    Transform target;
    public Transform spawnPoint;

    private Vector3[] bottleLocations; 
    private float throwBottleTime = 2f;
    private int bottleCount = 0;
    int maxBottles = 5;
    public GameObject bottle;

    [SerializeField]
    private float maxShotBottleTimer = .75f;
    private float shotBottleTimer;
    private float bufferTimer = 2f;
    private int bottlesShot = 0;

    public GameObject bullet;

    private float attackCoolDown = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.Find("player").transform;
        bottleLocations = new Vector3[6];
        shotBottleTimer = maxShotBottleTimer;
    }

    // Update is called once per frame
    void Update()
    {
        
        throwBottleTime -= Time.deltaTime;

        if(throwBottleTime < 0 && bottleCount < maxBottles){
            ThrowBottle();
            bottleLocations [bottleCount] = (target.position - transform.position).normalized;
            Debug.Log(bottleCount + "Count");
            
            bottleCount++;
            
            throwBottleTime = 2;

        }else if(bottleCount == maxBottles && bottlesShot != maxBottles){
            bufferTimer -= Time.deltaTime;
            if(bufferTimer < 0){
                shotBottleTimer -= Time.deltaTime;
                if(shotBottleTimer < 0){
                    ShootBottle();
                    Debug.Log(bottlesShot + "Shot");
                    bottlesShot++;
                    shotBottleTimer = maxShotBottleTimer;
                }
            }
        }else if(bottlesShot == maxBottles){
            attackCoolDown -= Time.deltaTime;
            //if(attackCoolDown < 0){
                bottleCount = 0;
                bottlesShot = 0;
                bufferTimer = 2f;
                throwBottleTime = 2;
                attackCoolDown = 3;
            //}
        }

        

    }

    void ThrowBottle(){
        GameObject bottleObj = Instantiate(bottle, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;

    }

    void ShootBottle(){
        GameObject bulletObj = Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
    }
    
    public Vector3 getLocation(){
        return bottleLocations[bottlesShot];
    }
}
