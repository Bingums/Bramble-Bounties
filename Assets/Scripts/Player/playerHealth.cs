using UnityEngine;
using Combat;

public class playerHealth : MonoBehaviour
{
    public int health = 100;
    

    void playerDamage(int damage){
        health -= damage;
        if(health <= 0){
            //To add a animation call a new method and call destroy there
           Destroy(gameObject);
        }
    }
}
