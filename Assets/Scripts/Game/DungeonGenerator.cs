using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int roomsToCreate = 15;
    public int gridSize = 8;

    [Header("Room Pools")]
    public RoomData startRoom;
    public RoomData bossRoom;
    public RoomData[] normalRooms;

    [Header("Room Size")]
    public float roomWidth = 21f;
    public float roomHeight = 15f;

    [Header("Player")]
    public GameObject player;

    // Tracks rooms by grid position
    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> roomPositions = new List<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        Vector2Int startPos = Vector2Int.zero;

        SpawnRoom(startRoom, startPos);
        roomPositions.Add(startPos);

        int attempts = 0;
        int maxAttempts = 500; // prevents infinite loops

        Vector2Int currentPos = startPos;

        for (int i = 1; i < roomsToCreate; i++)
        {
            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.LogWarning("Could not place all rooms without overlap!");
                break;
            }

            Vector2Int newPos = currentPos + GetRandomDirection();

            // Check for overlap
            if (spawnedRooms.ContainsKey(newPos))
            {
                i--; // retry this room
                continue;
            }

            RoomData randomRoom = normalRooms[Random.Range(0, normalRooms.Length)];
            SpawnRoom(randomRoom, newPos);
            roomPositions.Add(newPos);

            currentPos = newPos;
        }

        PlaceBossRoom(startPos);
        UpdateAllDoors();
        SpawnPlayer(startPos);
    }

    Vector2Int GetRandomDirection()
    {
        int dir = Random.Range(0, 4);
        if (dir == 0) return Vector2Int.up;
        if (dir == 1) return Vector2Int.down;
        if (dir == 2) return Vector2Int.left;
        return Vector2Int.right;
    }

    void PlaceBossRoom(Vector2Int startPos)
    {
        Vector2Int farthestRoom = startPos;
        float maxDistance = 0;

        foreach (Vector2Int pos in roomPositions)
        {
            float dist = Vector2Int.Distance(startPos, pos);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                farthestRoom = pos;
            }
        }

        // Replace farthest normal room with boss
        Destroy(spawnedRooms[farthestRoom]);
        SpawnRoom(bossRoom, farthestRoom);
    }

    void SpawnRoom(RoomData data, Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomWidth, gridPos.y * roomHeight, 0);
        GameObject room = Instantiate(data.prefab, worldPos, Quaternion.identity, transform);
        spawnedRooms[gridPos] = room;
    }

  void UpdateAllDoors()
{
    // Step 1: record what doors each room should have
    Dictionary<Room, (bool up, bool down, bool left, bool right)> doorMap = new Dictionary<Room, (bool, bool, bool, bool)>();

    foreach (var roomPair in spawnedRooms)
    {
        Vector2Int pos = roomPair.Key;
        Room room = roomPair.Value.GetComponent<Room>();

        bool hasUp = spawnedRooms.ContainsKey(pos + Vector2Int.up);
        bool hasDown = spawnedRooms.ContainsKey(pos + Vector2Int.down);
        bool hasLeft = spawnedRooms.ContainsKey(pos + Vector2Int.left);
        bool hasRight = spawnedRooms.ContainsKey(pos + Vector2Int.right);

        doorMap[room] = (hasUp, hasDown, hasLeft, hasRight);
    }

    // Step 2: apply doors per room
    foreach (var kvp in doorMap)
    {
        Room room = kvp.Key;
        var doors = kvp.Value;

        room.DisableAllDoors(); // start clean

        // Only enable one door per connection
        if (doors.up) room.EnableDoor(Room.Direction.Top);
        if (doors.down) room.EnableDoor(Room.Direction.Bottom);
        if (doors.left) room.EnableDoor(Room.Direction.Left);
        if (doors.right) room.EnableDoor(Room.Direction.Right);
    }
}
void SpawnPlayer(Vector2Int startPos)
{
    if (player == null) return;

    GameObject startRoomObj = spawnedRooms[startPos];
    Room room = startRoomObj.GetComponent<Room>();

    Transform spawn = room.GetPlayerSpawn();

}
}