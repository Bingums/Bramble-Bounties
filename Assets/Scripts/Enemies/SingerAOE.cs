using UnityEngine;
public class SingerAOE : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnTriggerEnter2D(Collider2D collision){
            
        GameObject player = GameObject.Find("player");
        if(collision.tag == "Player"){
            playerController other = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
            other.TakeDamage(10);
        }
    }
    
}
