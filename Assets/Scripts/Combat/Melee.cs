using UnityEngine;
using Combat;

public class Melee : MonoBehaviour
{
    public WeaponData data;
    public AudioClip attackSfx;
    public AudioSource audioSource;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.damage);
                audioSource.PlayOneShot(data.attackSFX);
            }
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
