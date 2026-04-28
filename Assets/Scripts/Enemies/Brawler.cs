using System.Collections;
using UnityEngine;

public class Brawler : MonoBehaviour
{
    SpriteRenderer brawlerRenderer;
    Color brawlerColor;
    Color damageColor = new Color(0.85f, 0.24f, 0.24f);

    void Awake()
    {
        brawlerRenderer = GetComponent<SpriteRenderer>();
        brawlerColor = brawlerRenderer.color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created 

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Melee") || collision.CompareTag("EnemyProjectile"))
        {
            brawlerRenderer.color = damageColor;
            
        }
        GameObject player = GameObject.Find("player");
        if(collision.tag == "Player"){
            playerController other = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
            other.TakeDamage(5);
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Melee") || collision.CompareTag("EnemyProjectile"))
        {
            StartCoroutine(Wait(0.5f));
            brawlerRenderer.color = brawlerColor;
        }
    }

    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
