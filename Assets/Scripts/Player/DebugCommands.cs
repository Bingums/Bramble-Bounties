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

        if (Input.GetKeyDown(KeyCode.F5))
        {
            LoadFloorFiveSelection();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            KillPlayer();
        }
    }

    private void LoadFloorFiveSelection()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("Cannot load floor 5 selection: GameManager not found.");
            return;
        }

        GameManager.Instance.DebugSetCurrentFloor(5);
        GameManager.Instance.LoadScene("Bounty Testing");
        Debug.Log("Debug loaded floor 5 bounty selection.");
    }
    
    private void KillAllEnemiesAndClearRoom()
    {
        if (EnemySpawnManager.Instance == null)
        {
            Debug.Log("Cannot clear room: EnemySpawnManager not found.");
            return;
        }

        int destroyedEnemyCount = EnemySpawnManager.Instance.DebugClearCurrentRoom();
        if (destroyedEnemyCount < 0)
        {
            Debug.Log("Cannot clear room: no current room.");
            return;
        }

        Debug.Log($"Debug cleared room. Destroyed {destroyedEnemyCount} enemies.");
    }

    private void KillPlayer()
    {
        if (playerController.Instance == null)
        {
            Debug.Log("Cannot kill player: playerController not found.");
            return;
        }

        playerController.Instance.TakeDamage(int.MaxValue);
        Debug.Log("Debug killed player.");
    }
}
