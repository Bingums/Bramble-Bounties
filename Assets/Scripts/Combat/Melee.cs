using UnityEngine;
using Combat;
using System.Data.Common;

public class Melee : MonoBehaviour
{
    public WeaponData data;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyData>().TakeDamage(data.damage);
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
