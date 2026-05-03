using UnityEngine;
public class SingerAOE : MonoBehaviour
{
    [SerializeField] private int damage = 12;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            collision.GetComponent<playerController>().TakeDamage(damage);
        }
         
    }
    
}
