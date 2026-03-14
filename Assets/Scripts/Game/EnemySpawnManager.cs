using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;

    [SerializeField] private GameObject[] enemyPrefabs;

    private int spawnRate = 6;

    private Room[] levelRooms;

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
        levelRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        StartSpawning();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnLoop()
    {
        while(true) {
            foreach(Room room in levelRooms)
            {
                if(!room.atCap())
                {
                    Transform[] spawnPoints = room.GetEnemySpawns();

                    foreach(Transform spawnPoint in spawnPoints)
                    {
                        GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                        Instantiate(enemy, spawnPoint);
                        room.IncreaseEnemyCount();
                    }
                }
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}
