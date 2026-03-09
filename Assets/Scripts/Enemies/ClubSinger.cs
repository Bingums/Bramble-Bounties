using UnityEngine;

public class ClubSinger : MonoBehaviour
{
    float aoeWarningTime = 2;
    float aoeTimer = 2;

    float attackTime = 1;

    bool aoeAttackFlag = false;
    int speed = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject aoeAttack = GameObject.Find("AOE Attack");
        GameObject warning = GameObject.Find("Warning");
        GameObject Singer = GameObject.Find("ClubSingerBoss");
        
        if(speed == 0){
            aoeWarningTime -= Time.deltaTime;
            if(aoeWarningTime < 0){
                transform.Find("AOE Attack").gameObject.SetActive(true);
                transform.Find("Warning").gameObject.SetActive(false);
                Debug.Log("Diabled");
                aoeAttackFlag = true;
            }
            
            
            if(aoeAttackFlag){
                attackTime -= Time.deltaTime;
            }
            if(aoeAttackFlag && attackTime < 0){
                transform.Find("AOE Attack").gameObject.SetActive(false);
                Singer.GetComponent<EnemyMovement>().moveSpeed = 2;
                speed = 2;
                aoeAttackFlag = false;
                aoeWarningTime = aoeTimer;
                attackTime = 1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        GameObject Singer = GameObject.Find("ClubSingerBoss");
        //GameObject warning = GameObject.Find("Warning");
        if(collision.tag == "Player"){
            if(Random.Range(0,7) < 1){
                Singer.GetComponent<EnemyMovement>().moveSpeed = 0;
                speed = 0;
                transform.Find("Warning").gameObject.SetActive(true);
                return;
            }
            if(Random.Range(0,5) < 1){
                
            }
        }
    }
}