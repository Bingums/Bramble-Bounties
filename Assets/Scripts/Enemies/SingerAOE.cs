using UnityEngine;
public class SingerAOE : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    void OnTriggerEnter2D(Collider2D collision){
            
        GameObject player = GameObject.Find("player");
        if(collision.tag == "Player"){
            playerHealth other = GameObject.FindGameObjectWithTag("Player").GetComponent<playerHealth>();
            other.playerDamage(10);
        }
    }
    
}
