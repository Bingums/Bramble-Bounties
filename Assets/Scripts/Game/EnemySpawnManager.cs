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
        if (GameManager.Instance.GetActiveBounty() != null)
            currentBounty = GameManager.Instance.GetActiveBounty();

        if (currentRoom.isBossRoom)
        {
            yield return SpawnBossRoom();
        }
        else
        {
            yield return SpawnNormalRoom();
        }
    }
    
    
    private IEnumerator SpawnNormalRoom(){
        currentRoom.ScaleWaves(currentBounty.ExtraWaves, currentBounty.ExtraEnemiesPerWave);
        while (currentRoom.currentWave < currentRoom.numWaves)
        {
            while (!currentRoom.AtCap())
            {
                hc.ShowIncomingWave(spawnRate);
                yield return new WaitForSeconds(spawnRate);
                
                Transform[] spawnPoints = currentRoom.GetEnemySpawns();
                
                foreach (Transform spawnPoint in spawnPoints)
                {
                    if (currentRoom.AtCap())
                        break;

                    GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                    GameObject spawnedEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);

                    EnemyController ec = spawnedEnemy.GetComponent<EnemyController>();
                    ec.ScaleStats(
                        currentBounty.HealthMultiplier,
                        currentBounty.AttackMultiplier,
                        currentBounty.MoveSpeedMultiplier
                    );

                    currentRoom.IncreaseEnemyCounts();
                }
            }

            while (currentRoom.enemyCount != 0)
                yield return null;
            
            currentRoom.ResetEnemyCounts();
            currentRoom.currentWave++;
        }

        ClearCurrentRoom(spawnAugment: true);
    }
    
    private IEnumerator SpawnBossRoom()
    {
        GameObject bossPrefab = GetBossPrefab();
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab not found for bounty: " + currentBounty.MinibossId);
            ClearCurrentRoom(spawnAugment: false);
            yield break;
        }
        
        Transform[] spawnPoints = currentRoom.GetEnemySpawns();
        Transform spawnPoint = spawnPoints.Length > 0 ? spawnPoints[0] : currentRoom.transform;

        GameObject spawnedBoss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

        EnemyController ec = spawnedBoss.GetComponent<EnemyController>();
        ec.ScaleStats(
            currentBounty.HealthMultiplier,
            currentBounty.AttackMultiplier,
            currentBounty.MoveSpeedMultiplier
        );

        if (hc != null)
        {
            hc.ShowBossHealth(ec);
        }

        currentRoom.IncreaseEnemyCounts();

        while (currentRoom.enemyCount != 0)
            yield return null;

        ClearCurrentRoom(spawnAugment: false);
    }

    private GameObject GetBossPrefab()
    {
        if (currentBounty == null) return null;

        switch (currentBounty.MinibossId)
        {
            case "0":
                return bossPrefabs[0];
            case "1":
                return bossPrefabs[1];
            case "2":
                return bossPrefabs[2];
            case "3":
                return bossPrefabs[3];
            default:
                return null;
        }
    }

    private void ClearCurrentRoom(bool spawnAugment)
    {
        currentRoom.isCleared = true;
        
        if(hc != null) hc.ShowRoomCleared();

        if (spawnAugment) SpawnAugment();
        
        currentRoom.LockDoors(false);
        currentRoom = null;
        
        Debug.Log("Room complete");
    }

    public void StartSpawning(Room room)
    {
        if (!Application.isPlaying) return;
        if (currentRoom != null) return;

        currentRoom = room;
        StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public int DebugClearCurrentRoom()
    {
        if (currentRoom == null)
        {
            return -1;
        }

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        StopAllCoroutines();
        currentRoom.ResetEnemyCounts();
        ClearCurrentRoom(spawnAugment: false);

        return enemies.Length;
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
            GameObject hudContainer = GameObject.Find("HUD Container");
            if (hudContainer != null)
            {
                hc = hudContainer.GetComponentInChildren<HUDController>();
            }

            yield return null;
        }
    }
}
