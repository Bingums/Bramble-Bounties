using UnityEngine;
using Combat;

public class Melee : MonoBehaviour
{
    public WeaponData data;
    
    private int damage;
    
    

    public void Initialize(int finalDamage)
    {
        damage = finalDamage;
    }

    public void Swing(Animator animator)
    {
        StopAllCoroutines();
        Debug.Log("Swing Animation");
        StartCoroutine(SwingRoutine(animator));
        //animator.SetBool("isSwinging", false);
    }

    private System.Collections.IEnumerator SwingRoutine(Animator animator)
    {
        /*
        float duration = 0.15f;
        float time = 0f;
        
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(180f, 0f, 315f);

        while (time < duration)
        {
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        */
        
        
        //transform.localRotation = startRotation;
        float duration = 0.20f;
        float time = 0f;
        
        animator.SetBool("isSwinging", true);
        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        
        animator.SetBool("isSwinging", false);
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            Debug.Log(damageable);
            damageable.TakeDamage(damage);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
