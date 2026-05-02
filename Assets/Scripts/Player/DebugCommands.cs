using UnityEngine;

public class DebugCommands : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GameManager.Instance == null)
            {
                Debug.Log("Selected bounty: GameManager not found.");
                return;
            }

            BountyData selectedBounty = GameManager.Instance.GetSelectedBounty();
            if (selectedBounty == null)
            {
                Debug.Log("Selected bounty: none");
                return;
            }

            Debug.Log($"Selected bounty: {selectedBounty.DisplayName} ({selectedBounty.BountyId})");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            KillAllEnemiesAndClearRoom();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject bountyBoard = GameObject.Find("BountySelectionPanel");
            if (bountyBoard == null)
            {
                Debug.Log("Bounty board not found.");
                return;
            }

            bountyBoard.SetActive(true);
            Debug.Log("Bounty board reopened.");
        }
    }
    
    private void KillAllEnemiesAndClearRoom()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        if (EnemySpawnManager.Instance == null)
        {
            Debug.Log("Cannot clear room: EnemySpawnManager not found.");
            return;
        }

        Room currentRoom = EnemySpawnManager.Instance.GetCurrentRoom();
        if (currentRoom == null)
        {
            Debug.Log("Cannot clear room: no current room.");
            return;
        }

        EnemySpawnManager.Instance.StopSpawning();

        currentRoom.ResetEnemyCounts();
        currentRoom.isCleared = true;
        currentRoom.LockDoors(false);

        Debug.Log($"Debug cleared room. Destroyed {enemies.Length} enemies.");
    }
}
