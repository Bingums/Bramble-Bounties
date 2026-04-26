using UnityEngine;

[CreateAssetMenu(fileName = "AugmentData", menuName = "Augments/Augment Data")]
public class AugmentData : ScriptableObject
{
    public string augmentName;
    public Rarity rarity; // possible rarities vary
    public bool isActive;
    public StatType statType; // may need to make array
    public ChangeType changeType; //same here
    public float changeValue; // and here
    public WeaponType weaponType;
}
