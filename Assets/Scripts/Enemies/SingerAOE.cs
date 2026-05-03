using UnityEngine;
public class SingerAOE : MonoBehaviour
{
    private int damage;
    
    public void InitializeDamage(int attack)
    {
        damage = attack;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            collision.GetComponentInParent<playerController>().TakeDamage(damage);
        }
         
    }
    
}
