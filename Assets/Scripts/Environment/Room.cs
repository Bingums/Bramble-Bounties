using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    private int enemyCap = 8;
    private int curEnemyCount = 0;
    private bool isCleared = false;

    // Door & Wall references
    public GameObject rightDoor, leftDoor, topDoor, bottomDoor;
    public GameObject rightWall, leftWall, topWall, bottomWall;

    // Enum for directions
    public enum Direction { Right, Left, Top, Bottom }

    // -------------------------
    // Enemy / Room Methods
    // -------------------------
    public bool atCap() => curEnemyCount >= enemyCap;
    public bool isRoomCleared() => isCleared;
    public void IncreaseEnemyCount() => curEnemyCount++;
    public void DecreaseEnemyCount() => curEnemyCount--;
    public Transform[] GetSpawnPoints() => spawnPoints;

    // -------------------------
    // Door / Wall Methods
    // -------------------------
    public void EnableDoor(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right:
                if (rightDoor != null) rightDoor.SetActive(true);
                if (rightWall != null) rightWall.SetActive(false);
                Debug.Log($"{name}: Enabled Right Door, Right Wall Off");
                break;
            case Direction.Left:
                if (leftDoor != null) leftDoor.SetActive(true);
                if (leftWall != null) leftWall.SetActive(false);
                Debug.Log($"{name}: Enabled Left Door, Left Wall Off");
                break;
            case Direction.Top:
                if (topDoor != null) topDoor.SetActive(true);
                if (topWall != null) topWall.SetActive(false);
                Debug.Log($"{name}: Enabled Top Door, Top Wall Off");
                break;
            case Direction.Bottom:
                if (bottomDoor != null) bottomDoor.SetActive(true);
                if (bottomWall != null) bottomWall.SetActive(false);
                Debug.Log($"{name}: Enabled Bottom Door, Bottom Wall Off");
                break;
        }
    }

    public void DisableDoor(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right:
                if (rightDoor != null) rightDoor.SetActive(false);
                if (rightWall != null) rightWall.SetActive(true);
                Debug.Log($"{name}: Disabled Right Door, Right Wall On");
                break;
            case Direction.Left:
                if (leftDoor != null) leftDoor.SetActive(false);
                if (leftWall != null) leftWall.SetActive(true);
                Debug.Log($"{name}: Disabled Left Door, Left Wall On");
                break;
            case Direction.Top:
                if (topDoor != null) topDoor.SetActive(false);
                if (topWall != null) topWall.SetActive(true);
                Debug.Log($"{name}: Disabled Top Door, Top Wall On");
                break;
            case Direction.Bottom:
                if (bottomDoor != null) bottomDoor.SetActive(false);
                if (bottomWall != null) bottomWall.SetActive(true);
                Debug.Log($"{name}: Disabled Bottom Door, Bottom Wall On");
                break;
        }
    }

    public void DisableAllDoors()
    {
        DisableDoor(Direction.Right);
        DisableDoor(Direction.Left);
        DisableDoor(Direction.Top);
        DisableDoor(Direction.Bottom);
        Debug.Log($"{name}: Disabled All Doors");
    }
}