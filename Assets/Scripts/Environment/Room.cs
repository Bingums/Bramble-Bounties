using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    private int enemyCap = 16;
    private int curEnemyCount = 0;
    private bool isCleared = false;
    public GameObject rightDoor;
    public GameObject leftDoor;
    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject rightWall;
    public GameObject leftWall;
    public GameObject topWall;
    public GameObject bottomWall;
    

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


   
    // Enum for directions
    public enum Direction
    {
        Right,
        Left,
        Top,
        Bottom
    }
    
    // Enable a specific door and disable its corresponding wall
   public void EnableDoor(Direction direction)
{
    switch (direction)
    {
        case Direction.Right:
            if (rightDoor != null) rightDoor.SetActive(true);
            if (rightWall != null) rightWall.SetActive(false);
            break;
        case Direction.Left:
            if (leftDoor != null) leftDoor.SetActive(true);
            if (leftWall != null) leftWall.SetActive(false);
            break;
        case Direction.Top:
            if (topDoor != null) topDoor.SetActive(true);
            if (topWall != null) topWall.SetActive(false);
            break;
        case Direction.Bottom:
            if (bottomDoor != null) bottomDoor.SetActive(true);
            if (bottomWall != null) bottomWall.SetActive(false);
            break;
    }
}

public void EnableWall(Direction direction)
{
    switch (direction)
    {
        case Direction.Right:
            if (rightDoor != null) rightDoor.SetActive(false);
            if (rightWall != null) rightWall.SetActive(true);
            break;
        case Direction.Left:
            if (leftDoor != null) leftDoor.SetActive(false);
            if (leftWall != null) leftWall.SetActive(true);
            break;
        case Direction.Top:
            if (topDoor != null) topDoor.SetActive(false);
            if (topWall != null) topWall.SetActive(true);
            break;
        case Direction.Bottom:
            if (bottomDoor != null) bottomDoor.SetActive(false);
            if (bottomWall != null) bottomWall.SetActive(true);
            break;
    }
}
    
    // Helper method to set door/wall states
    private void SetDoorWall(GameObject door, GameObject wall, bool doorEnabled)
    {
        if (door != null)
            door.SetActive(doorEnabled);
        
        if (wall != null)
            wall.SetActive(!doorEnabled);
    }
    
    // Disable all doors
    public void DisableAllDoors()
    {
        SetDoorWall(rightDoor, rightWall, false);
        SetDoorWall(leftDoor, leftWall, false);
        SetDoorWall(topDoor, topWall, false);
        SetDoorWall(bottomDoor, bottomWall, false);
    }
    
    
    // Optional: Check if a direction has a door
    public bool HasDoor(Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return rightDoor != null && rightDoor.activeSelf;
            case Direction.Left:
                return leftDoor != null && leftDoor.activeSelf;
            case Direction.Top:
                return topDoor != null && topDoor.activeSelf;
            case Direction.Bottom:
                return bottomDoor != null && bottomDoor.activeSelf;
            default:
                return false;
        }
    }
    
}