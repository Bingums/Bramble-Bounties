using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    private int enemyCap = 16;
    private int curEnemyCount = 0;
    private bool isCleared = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool atCap()
    {
        if(curEnemyCount >= enemyCap) return true;
        return false;
    }

    public bool isRoomCleared()
    {
        return isCleared;
    }

    public void IncreaseEnemyCount()
    {
        Debug.Log(curEnemyCount);
        curEnemyCount++;
    }

    public void DecreaseEnemyCount()
    {
        curEnemyCount--;
    }

    public Transform[] GetSpawnPoints()
    {
        return spawnPoints;
    }
}
