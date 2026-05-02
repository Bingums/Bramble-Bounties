using System.Collections;
using UnityEngine;

public class Brawler : EnemyController
{
    SpriteRenderer brawlerRenderer;
    Color brawlerColor;
    Color damageColor = new Color(0.85f, 0.24f, 0.24f);
    private AudioSource audioSource;
    public AudioClip punchSFX;
    
    protected override void Awake()
    {
        base.Awake();
        brawlerRenderer = GetComponent<SpriteRenderer>();
        brawlerColor = brawlerRenderer.color;
        audioSource = GetComponent<AudioSource>();
        scoreValue = 50;
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Melee"))
        {
            brawlerRenderer.color = damageColor;
        }
        
        if(collision.CompareTag("Player")){
            collision.GetComponentInParent<playerHealth>().playerDamage(attack);
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
                                           || collision.CompareTag("Melee")) {
            StartCoroutine(Recolor(0.2f));
        }
    }

    IEnumerator Recolor(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        brawlerRenderer.color = brawlerColor;
    }
}
