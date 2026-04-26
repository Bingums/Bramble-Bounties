using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;

    [SerializeField] private GameObject[] enemyPrefabs;

    public Transform player;
    private int spawnRate = 2; //seconds
    private int newWaveSpawnRate = 1; //(2 + 1)

    private Room currentRoom;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator SpawnLoop()
    {
        while(currentRoom.currentWave < currentRoom.numWaves)
        {
            while(!currentRoom.AtCap())
            {
                yield return new WaitForSeconds(spawnRate);
                
                Transform[] spawnPoints = currentRoom.GetEnemySpawns();
                
                foreach (Transform spawnPoint in spawnPoints)
                {
                    if (!currentRoom.AtCap())
                    {
                        GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                        Instantiate(enemy, spawnPoint.position, Quaternion.identity);
                        currentRoom.IncreaseEnemyCounts();
                    }
                }
            }

            while(currentRoom.enemyCount != 0) 
            {
                yield return null;
            }
            
            currentRoom.ResetEnemyCounts();
            
            currentRoom.currentWave++;
            yield return new WaitForSeconds(newWaveSpawnRate);
        }
        
        currentRoom.isCleared = true;
        currentRoom.LockDoors(false);
        currentRoom = null;
        Debug.Log("All waves complete");
        StopSpawning();
        yield break; // gets rid of extra iteration
    }

    public void StartSpawning(Room room)
    {
        if (!Application.isPlaying) return;
        currentRoom = room;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }
    
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
}
