using System.Collections;
using UnityEngine;
using Combat;
using UnityEngine.Diagnostics;

public class BasicShooter : EnemyController
{
    //Shooting Var
    public float firingDistance;
    private float timer = 1;
    private float bulletTime;
    public GameObject bullet;
    public Transform spawnPoint;

    SpriteRenderer shooterRenderer;
    Color shooterColor;
    Color damageColor = new Color(0.85f, 0.24f, 0.24f);
    
    private AudioSource audioSource;
    public AudioClip plasmaGunSFX;

    protected override void Awake(){
        base.Awake();
        shooterRenderer = GetComponent<SpriteRenderer>();
        shooterColor = shooterRenderer.color;
        audioSource = GetComponent<AudioSource>();
    }

    protected override void Update()
    {
        base.Update();
        if(target){
            if(distance < firingDistance) {
                moveSpeed = 0;
                ShootGun();
            } else if(distance > firingDistance) {
                moveSpeed = 2;
            }
        }
    }
    
    private void ShootGun(){
        bulletTime -= Time.deltaTime;
        if(bulletTime > 0) return;
        bulletTime = timer;

        GameObject shotBullet = Instantiate(bullet, spawnPoint.transform.position, spawnPoint.transform.rotation);
        shotBullet.GetComponent<EnemyBullet>().InitializeEnemyBullet(attack, bulletSpeed, range, moveDirection);
        audioSource.PlayOneShot(plasmaGunSFX);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerProjectile")
            || collision.CompareTag("Weapon"))
        {
            if(collision.CompareTag("Player"))
            {
                collision.GetComponentInParent<playerController>().TakeDamage(attack);
            }
            
            shooterRenderer.color = damageColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        StartCoroutine(Recolor(0.2f));
    }

    IEnumerator Recolor(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        shooterRenderer.color = shooterColor;
    }
}
