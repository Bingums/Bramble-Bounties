using UnityEngine;
using Combat;

public class playerHealth : MonoBehaviour
{
    public int health;
    

    public void playerDamage(int damage){
        health -= damage;
        Debug.Log("I AM HERE " + damage);
        if(health <= 0){
            //To add a animation call a new method and call destroy there
            FindFirstObjectByType<PauseMenu>().GameOver();
        }
    }
}
