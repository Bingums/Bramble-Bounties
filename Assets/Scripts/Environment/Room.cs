using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] enemySpawnPoints; // first 4 points for enemies
    [SerializeField] private Transform playerSpawnPoint;   // dedicated player spawn

    private int enemyCap = 8;
    private int curEnemyCount = 0;
    private bool isCleared = false;

    // Door & Wall references
    public GameObject rightDoor, leftDoor, topDoor, bottomDoor;
    public GameObject rightWall, leftWall, topWall, bottomWall;

    public enum Direction { Right, Left, Top, Bottom }

    // -------------------------
    // Enemy / Room Methods
    // -------------------------
    public bool atCap() => curEnemyCount >= enemyCap;
    public bool isRoomCleared() => isCleared;
    public void IncreaseEnemyCount() => curEnemyCount++;
    public void DecreaseEnemyCount() => curEnemyCount--;

    public Transform[] GetEnemySpawns() => enemySpawnPoints;

    // -------------------------
    // Player Spawn Getter
    // -------------------------
    public Transform GetPlayerSpawn()
    {
        if (playerSpawnPoint != null)
            return playerSpawnPoint;

        // fallback: slightly above room center
        Transform temp = new GameObject("AutoPlayerSpawn").transform;
        temp.position = transform.position + Vector3.up * 1f;
        temp.parent = transform;
        return temp;
    }

    // -------------------------
    // Door / Wall Methods
    // -------------------------
    public void EnableDoor(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right: if (rightDoor) rightDoor.SetActive(true); if (rightWall) rightWall.SetActive(false); break;
            case Direction.Left: if (leftDoor) leftDoor.SetActive(true); if (leftWall) leftWall.SetActive(false); break;
            case Direction.Top: if (topDoor) topDoor.SetActive(true); if (topWall) topWall.SetActive(false); break;
            case Direction.Bottom: if (bottomDoor) bottomDoor.SetActive(true); if (bottomWall) bottomWall.SetActive(false); break;
        }
    }

    public void DisableDoor(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right: if (rightDoor) rightDoor.SetActive(false); if (rightWall) rightWall.SetActive(true); break;
            case Direction.Left: if (leftDoor) leftDoor.SetActive(false); if (leftWall) leftWall.SetActive(true); break;
            case Direction.Top: if (topDoor) topDoor.SetActive(false); if (topWall) topWall.SetActive(true); break;
            case Direction.Bottom: if (bottomDoor) bottomDoor.SetActive(false); if (bottomWall) bottomWall.SetActive(true); break;
        }
    }

    public void DisableAllDoors()
    {
        DisableDoor(Direction.Right);
        DisableDoor(Direction.Left);
        DisableDoor(Direction.Top);
        DisableDoor(Direction.Bottom);
    }

    public void SetDoors(bool up, bool down, bool left, bool right)
    {
        if (up) EnableDoor(Direction.Top); else DisableDoor(Direction.Top);
        if (down) EnableDoor(Direction.Bottom); else DisableDoor(Direction.Bottom);
        if (left) EnableDoor(Direction.Left); else DisableDoor(Direction.Left);
        if (right) EnableDoor(Direction.Right); else DisableDoor(Direction.Right);
    }
}