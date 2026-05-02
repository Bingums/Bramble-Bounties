using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] bossPrefabs;
    
    [SerializeField] private AugmentData[] augments;
    [SerializeField] public GameObject ammoPickup;
    [SerializeField] public GameObject healthPickup;

    public Transform player;
    private int spawnRate = 3; //seconds

    public BountyData currentBounty;
    private Room currentRoom;
    public HUDController hc;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        StartCoroutine(WaitForHUD());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    
    private IEnumerator SpawnLoop()
    {
        if(GameManager.Instance.GetSelectedBounty() != null)
            currentBounty = GameManager.Instance.GetSelectedBounty();
        currentRoom.ScaleWaves(currentBounty.ExtraWaves, currentBounty.ExtraEnemiesPerWave);
        while(currentRoom.currentWave < currentRoom.numWaves)
        {
            while(!currentRoom.AtCap())
            {
                hc.ShowIncomingWave(spawnRate);
                yield return new WaitForSeconds(spawnRate);
                
                Transform[] spawnPoints = currentRoom.GetEnemySpawns();
                
                foreach (Transform spawnPoint in spawnPoints)
                {
                    if (!currentRoom.AtCap())
                    {
                        GameObject enemy = null;
                        if (!currentRoom.isBossRoom) 
                            enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                        else
                        {
                            if (currentBounty == null)
                                continue;

                            switch (currentBounty.MinibossId)
                            {
                                case "Cyclops":
                                    enemy = bossPrefabs[1];
                                    break;
                                case "Medusa":
                                    enemy = bossPrefabs[2];
                                    break;
                                case "Wimp":
                                    enemy = bossPrefabs[0];
                                    break;
                                case "Kingpin":
                                    enemy = bossPrefabs[4];
                                    break;
                            }
                        }
                        GameObject spawnedEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
                        EnemyController ec = spawnedEnemy.GetComponent<EnemyController>();
                        ec.ScaleStats(currentBounty.HealthMultiplier, 
                                       currentBounty.AttackMultiplier,
                                       currentBounty.MoveSpeedMultiplier);
                        currentRoom.IncreaseEnemyCounts();
                    }
                }
            }

            while(currentRoom.enemyCount != 0) 
                yield return null;
            
            currentRoom.ResetEnemyCounts();
            
            currentRoom.currentWave++;
        }
        
        currentRoom.isCleared = true;
        hc.ShowRoomCleared();
        SpawnAugment();
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
    
    private void SpawnAugment()
    {
        if (augments == null || augments.Length == 0) return;
    
        AugmentData augment = augments[Random.Range(0, augments.Length)];
        GameObject pickup = Instantiate(augment.pickupPrefab, currentRoom.transform.position, Quaternion.identity);
        pickup.GetComponent<AugmentPickup>().data = augment;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }
    
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    
    IEnumerator WaitForHUD()
    {
        while (hc == null)
        {
            hc = GameObject.Find("HUD Container").GetComponentInChildren<HUDController>();
            yield return null;
        }
    }
}
