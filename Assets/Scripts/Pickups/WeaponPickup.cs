using System;
using UnityEngine;


//Script to handle player picking up weapons
//This should destroy the overworld version of the weapon when the player collides with it
public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerCombat player = other.gameObject.GetComponent<playerCombat>();

        if (player != null)
        {
            player.EquipWeapon(weaponData);
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
 
}
