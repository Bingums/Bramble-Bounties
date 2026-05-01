using UnityEngine;

public class Boss : MonoBehaviour
{

    public float moveSpeed = 2f;
    //public float distance;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    private Vector2 playerLocation;

    private int randAttack = 0;
    private bool playerContactFlag = false;

    private float timer = 0;

    Vector2 chargeVect;

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
                randAttack = 2;//Random.Range(0,3);
                timer = 0;


            }else if(randAttack == 1){//Charge Attack
                timer -= Time.deltaTime;
                if(timer > -.35f){
                    moveSpeed = 2;
                    moveDirection = (target.position - transform.position).normalized;
                    rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
                }else if(timer > -2){
                    moveSpeed = 6;
                    rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed ;

                }else{
                    moveSpeed = 0;
                    randAttack = -1;

                }
            
            
            }else if(randAttack == 2){//Fan bullets
                
            }else if(randAttack == 3 ){//Third attack if i can think of one
                
            }else{//Boss moves towards the player like a sort of cooldown I guess idk

            }
            }
        
    }
}
