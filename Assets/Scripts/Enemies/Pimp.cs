using System.Collections;
using UnityEngine;
using Combat;
using UnityEngine.Diagnostics;

public class Pimp : EnemyController
{
    //Shooting Var
    public float firingDistance;
    private float timer = .5f;
    private float bulletTime;
    private bool retreating;
    public GameObject bullet;
    public Transform bulletSpawnPoint;

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
        scoreValue = 2000;
    }

    protected override void Update()
    {
        if(target){
            distance = Vector2.Distance(transform.position, target.position);
            //Debug.Log("Distance: " + distance + " | Firing Distance: " + firingDistance);

            if (retreating && distance < firingDistance)
                return;
            else
                retreating = false;
            
            if (distance <= (firingDistance/2f))
            {
                retreating = true;
                // rb.linearVelocity = Vector2.zero;
                // rb.linearDamping = 10000f;
                moveDirection = (transform.position - target.position).normalized;
                moveSpeed = 2;
            } else if(distance <= firingDistance) {
                moveDirection = Vector2.zero;
                moveSpeed = 0;
                ShootGun();
            } else if(distance > firingDistance) {
                moveDirection = (target.position - transform.position).normalized;
                moveSpeed = 2;
            }
        }
        
        animator.SetFloat("TargetX", moveDirection.x);
        animator.SetBool("isMoving", rb.linearVelocity.magnitude > 0.1f);
    }
    
    private void ShootGun(){
        bulletTime -= Time.deltaTime;
        if (bulletTime > 0)
        {
            animator.SetTrigger("waiting");
            return;
        }
        bulletTime = timer;

        animator.SetTrigger("attacking");
        Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
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