using UnityEngine;
public class SingerAOE : MonoBehaviour
{
    [SerializeField] private int damage = 12;
    //private int damage;
    
    // public void InitializeDamage(int attack)
    // {
    //     damage = attack;
    // }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            collision.GetComponent<playerController>().TakeDamage(damage);
        }
         
    }
    
}
