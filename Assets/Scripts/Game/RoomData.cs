using UnityEngine;

[System.Serializable]
public class RoomData
{
    public GameObject prefab;
    public int doorCount;
    public bool allowsVerticalTwoDoor;

    [Tooltip("Use bitmask to indicate which doors exist: Right=1, Left=2, Top=4, Bottom=8")]
    public int doorMask; // NEW
}