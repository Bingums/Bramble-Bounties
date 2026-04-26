using System;
using System.Collections.Generic;
using UnityEngine;


public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance;
    
    // each index in equippedAugments corresponds to a certain slot
    private List<AugmentData> equippedAugments = new List<AugmentData>();
    // technically the same for unused but wouldnt matter as much
    // itll be displayed how its listed and any new augment goes to the back
    // prob have it displayed as most recent for ease of use though (traversing array backwards)
    private List<AugmentData> unusedAugments = new List<AugmentData>();
    // not sure if i need this yet
    private playerController pc;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        pc = GetComponent<playerController>();
    }
    
    // check if augment is equipped
    
    // equip an augment in certain slot
    // take in augment and slot num
    // set isActive and call applyAugment
    
    // deequip an augment in certain slot
    // take in augment and slot num
    // set isActive and call applyAugment
}
