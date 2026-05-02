using UnityEngine;

[CreateAssetMenu(fileName = "AugmentData", menuName = "Augments/Augment Data")]
public class AugmentData : ScriptableObject
{
    public Sprite augmentSprite;
    public Color augmentColor;
    public string augmentName;
    [TextArea(3, 5)] public string description;
    public int rarity;
    // public bool isActive;
    public StatModifier[] statModifiers;
    public WeaponType weaponType;
    // unique modifier for unique rarity
    public GameObject pickupPrefab;
    
    public AugmentData GetCopy()
    {
        AugmentData copy = CreateInstance<AugmentData>();
        copy.augmentSprite = pickupPrefab.GetComponent<SpriteRenderer>().sprite;
        copy.augmentColor = pickupPrefab.GetComponent<SpriteRenderer>().color;
        copy.augmentName = augmentName;
        copy.description = description;
        copy.rarity = rarity;
        copy.weaponType = weaponType;
        copy.pickupPrefab = pickupPrefab;
    
        copy.statModifiers = new StatModifier[statModifiers.Length];
        for (int i = 0; i < statModifiers.Length; i++)
        {
            copy.statModifiers[i] = new StatModifier
            {
                statType = statModifiers[i].statType,
                changeType = statModifiers[i].changeType,
                changeValue = statModifiers[i].changeValue
            };
        }
    
        return copy;
    }
}

[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public ChangeType changeType;
    public float changeValue;
}