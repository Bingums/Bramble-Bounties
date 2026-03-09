using UnityEngine;

public class Pimp : MonoBehaviour
{
    public GameObject enemyShooter, enemyBrawler, enemyDrunk;
    private GameObject player;

    private float vect1, vect2, vect3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake(){
        PimpManager.slap += RunToPlayer;
    }
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target){
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction; 
        }
    }
    void RunToPlayer(){
        target = GameObject.Find("player").transform;
    }
    void RunsAway(){

        vect1 = Random.Range(-6.0f,6.0f);
        vect2 = Random.Range(-3.5f,3.5f);
        vect3 = 0;
    }
    void EnemySpawner(){

    }
    void OnTriggerEnter2D(){
        //SLAP A HOE
    }
}
