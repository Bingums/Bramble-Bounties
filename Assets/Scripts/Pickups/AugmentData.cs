using UnityEngine;

[CreateAssetMenu(fileName = "AugmentData", menuName = "Augments/Augment Data")]
public class AugmentData : ScriptableObject
{
    public string augmentName;
    public string description;
    public int rarity;
    public RarityModifier[] possibleRarities;
    // public bool isActive;
    public StatModifier[] statModifiers;
    public WeaponType weaponType;
}

[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public ChangeType changeType;
    public float changeValue;
}

[System.Serializable]
public class RarityModifier
{
    public Rarity rarity;
    public float valueMultiplier;
}