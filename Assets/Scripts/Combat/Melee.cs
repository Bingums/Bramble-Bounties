using UnityEngine;
using Combat;

public class Melee : MonoBehaviour
{
    public MeleeWeaponData data;
    
    private int damage;
    
    

    public void Initialize(int finalDamage)
    {
        damage = finalDamage;
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
