using UnityEngine;

public class BurningPuddle : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            collision.GetComponent<PlayerState>().TakeDamage(12);
        }
         
    }
}
