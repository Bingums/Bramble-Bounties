using System;
using UnityEngine;

//Script to handle player picking up weapons
//This should destroy the overworld version of the weapon when the player collides with it
public class WeaponPickup : MonoBehaviour, IInteractable
{
    public WeaponData weapon;
    public Vector3 location;

    void Awake()
    {
        location = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
 
    public void Interact()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerController pc = player.GetComponent<playerController>();
        WeaponData[] weapons = pc.weapons;
        int curSlot = pc.curSlot;

        if(weapon.isMelee)
        {
            WeaponData prevMelee = weapons[3];
            weapons[2] = weapon;
            Instantiate(prevMelee.weaponPickup, location, Quaternion.identity);
        } else if(curSlot == 2)
        {
            Debug.Log("slot 3 is only for melee");
        } else
        {
            WeaponData prevGun = weapons[curSlot];
            weapons[curSlot] = weapon;
            Instantiate(prevGun.weaponPickup, location, Quaternion.identity);
        }
    }
}
