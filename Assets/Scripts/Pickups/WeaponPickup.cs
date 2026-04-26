using System;
using UnityEngine;

//Script to handle player picking up weapons
//This should destroy the overworld version of the weapon when the player collides with it
public class WeaponPickup : MonoBehaviour, IInteractable
{
    public playerController pc;
    public Weapon weapon;
    public Vector3 location;

    void Awake()
    {
        location = transform.position;
    }

    public void Interact(GameObject interactor)
    {
        pc = interactor.GetComponent<playerController>();
        PickupWeapon();
    }

    public void PickupWeapon()
    {
        Weapon[] weapons = pc.weapons;
        int curSlot = pc.curSlot;

        if(weapon.baseData.isMelee)
        {
            Weapon prevMelee = weapons[3];
            weapons[2] = weapon;
            Instantiate(prevMelee.baseData.weaponPickup, location, Quaternion.identity);
        } else if(curSlot == 2)
        {
            Debug.Log("slot 3 is only for melee");
        } else
        {
            Weapon prevGun = weapons[curSlot];
            weapons[curSlot] = weapon;
            Instantiate(prevGun.baseData.weaponPickup, location, Quaternion.identity);
        }
    }
}
