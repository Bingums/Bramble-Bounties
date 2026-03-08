using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int gridSize = 10;
    public int roomsToCreate = 15;

    [Header("Room Prefabs")]
    public RoomData[] roomPrefabs;

    [Header("Tilemap Room Size")]
    public float roomWidth = 20f;
    public float roomHeight = 20f;

    [Header("Seed Settings")]
    public int seed = 0;
    public bool useSeed = false;
    private bool[,] dungeonGrid;
    private Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();
    private HashSet<string> processedConnections = new HashSet<string>();
    void Start()
    {
        dungeonGrid = new bool[gridSize, gridSize];

        if (useSeed)
            Random.InitState(seed);

        GenerateDungeon();
        SpawnRooms();
    }

    // ------------------------------------------------
    // Dungeon Generation
    // ------------------------------------------------

 void GenerateDungeon()
{
    Vector2Int start = new Vector2Int(gridSize / 2, gridSize / 2);

    dungeonGrid[start.x, start.y] = true;

    List<Vector2Int> frontier = new List<Vector2Int>();
    frontier.Add(start);

    int createdRooms = 1;
    int safetyCounter = 0;
    int maxIterations = gridSize * gridSize * 20;

    while (createdRooms < roomsToCreate && safetyCounter < maxIterations)
    {
        safetyCounter++;

        if (frontier.Count == 0)
            break;

        // Pick a random frontier cell
        Vector2Int baseRoom = frontier[Random.Range(0, frontier.Count)];

        // Get all possible directions (neighbors that are empty and valid)
        List<Vector2Int> possibleDirections = new List<Vector2Int>();
        
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = baseRoom + dir;
            if (IsValidGridPosition(neighbor) && !dungeonGrid[neighbor.x, neighbor.y])
            {
                // Optional: Add bias against creating straight lines
                // This checks if we're continuing in the same direction
                if (Random.value < 0.7f || frontier.Count <= 2)
                {
                    possibleDirections.Add(dir);
                }
            }
        }

        if (possibleDirections.Count == 0)
        {
            // No valid neighbors, remove from frontier
            frontier.Remove(baseRoom);
            continue;
        }

        // Pick a random direction
        Vector2Int direction = possibleDirections[Random.Range(0, possibleDirections.Count)];
        Vector2Int newPos = baseRoom + direction;

        // Place the new room
        dungeonGrid[newPos.x, newPos.y] = true;
        frontier.Add(newPos);
        createdRooms++;
    }

    // Debug output
    string debug = "Filled cells:\n";
    for (int x = 0; x < gridSize; x++)
    {
        for (int y = 0; y < gridSize; y++)
        {
            if (dungeonGrid[x, y])
                debug += $"({x}, {y}) ";
        }
    }
    Debug.Log(debug);
}

    // ------------------------------------------------
    // Room Spawning
    // ------------------------------------------------

   void SpawnRooms()
{
    // Calculate offset to center the dungeon
    float totalWidth = (gridSize - 1) * roomWidth;
    float totalHeight = (gridSize - 1) * roomHeight;
    Vector3 dungeonOffset = new Vector3(-totalWidth/2, -totalHeight/2, 0); // Fixed: should be negative
    
    Debug.Log($"=== ROOM SPAWNING DEBUG ===");
    Debug.Log($"gridSize: {gridSize}, roomsToCreate: {roomsToCreate}");
    Debug.Log($"roomWidth: {roomWidth}, roomHeight: {roomHeight}");
    Debug.Log($"dungeonOffset: {dungeonOffset}");
    
    // Clear previous data
    spawnedRooms.Clear();
    processedConnections.Clear();
    
    // FIRST PASS: Spawn all rooms and store them
    for (int x = 0; x < gridSize; x++)
    {
        for (int y = 0; y < gridSize; y++)
        {
            if (!dungeonGrid[x, y])
                continue;

            Vector3 position = new Vector3(
                x * roomWidth,
                y * roomHeight,
                0f
            ) + dungeonOffset;
            
            Debug.Log($"Grid ({x},{y}) -> World Position: {position}");

            int requiredMask = GetRequiredDoorMask(x, y);
            List<RoomData> validRooms = new List<RoomData>();

            foreach (var roomData in roomPrefabs)
            {
                if ((roomData.doorMask & requiredMask) == requiredMask)
                    validRooms.Add(roomData);
            }

            if (validRooms.Count == 0)
            {
                Debug.LogWarning($"No valid room for mask {requiredMask} at ({x},{y})");
                continue;
            }

            RoomData selectedRoom = validRooms[Random.Range(0, validRooms.Count)];
            
            GameObject roomObj = Instantiate(selectedRoom.prefab, position, Quaternion.identity);
            roomObj.name = $"Room_{x}_{y}";

            Room room = roomObj.GetComponent<Room>();
            if (room != null)
            {
                spawnedRooms[new Vector2Int(x, y)] = room; // Store in dictionary
            }
        }
    }
    
    // SECOND PASS: Configure doors for all rooms
    foreach (var kvp in spawnedRooms)
    {
        Vector2Int pos = kvp.Key;
        Room room = kvp.Value;
        
        ConfigureRoomDoorsWithNeighbors(room, pos.x, pos.y);
    }
}
    // ------------------------------------------------
    // Door Configuration
    // ------------------------------------------------

    void ConfigureRoomDoorsWithNeighbors(Room room, int x, int y)
{
    // Clear all doors/walls first
    room.DisableAllDoors();
    
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1),  // Top
        new Vector2Int(0, -1)  // Bottom
    };
    
    Room.Direction[] roomDirections = new Room.Direction[]
    {
        Room.Direction.Right,
        Room.Direction.Left,
        Room.Direction.Top,
        Room.Direction.Bottom
    };
    
    for (int i = 0; i < directions.Length; i++)
    {
        Vector2Int neighborPos = new Vector2Int(x + directions[i].x, y + directions[i].y);
        
        if (spawnedRooms.ContainsKey(neighborPos))
        {
            // Only enable door if we haven't already processed this connection
            // This ensures each connection has exactly ONE door
            if (!IsConnectionProcessed(x, y, neighborPos))
            {
                room.EnableDoor(roomDirections[i]);
                MarkConnectionProcessed(x, y, neighborPos);
            }
        }
        else
        {
            room.EnableWall(roomDirections[i]);
        }
    }
}

bool IsConnectionProcessed(int x1, int y1, Vector2Int pos2)
{
    string id1 = $"{x1},{y1}-{pos2.x},{pos2.y}";
    string id2 = $"{pos2.x},{pos2.y}-{x1},{y1}";
    return processedConnections.Contains(id1) || processedConnections.Contains(id2);
}

void MarkConnectionProcessed(int x1, int y1, Vector2Int pos2)
{
    string id = $"{x1},{y1}-{pos2.x},{pos2.y}";
    processedConnections.Add(id);
}

    Vector2Int GetRandomEmptyDirection(Vector2Int pos)
    {
        List<Vector2Int> options = new List<Vector2Int>();

        if (!IsRoomAt(pos.x + 1, pos.y)) options.Add(Vector2Int.right);
        if (!IsRoomAt(pos.x - 1, pos.y)) options.Add(Vector2Int.left);
        if (!IsRoomAt(pos.x, pos.y + 1)) options.Add(Vector2Int.up);
        if (!IsRoomAt(pos.x, pos.y - 1)) options.Add(Vector2Int.down);

        if (options.Count == 0)
            return Vector2Int.zero;

        return options[Random.Range(0, options.Count)];
    }

    bool IsRoomAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridSize || y >= gridSize)
            return false;

        return dungeonGrid[x, y];
    }

    int GetRequiredDoorMask(int x, int y)
    {
        int mask = 0;

        if (IsRoomAt(x + 1, y)) mask |= 1;
        if (IsRoomAt(x - 1, y)) mask |= 2;
        if (IsRoomAt(x, y + 1)) mask |= 4;
        if (IsRoomAt(x, y - 1)) mask |= 8;

        return mask;
    }
    bool IsValidGridPosition(Vector2Int pos)
{
    return pos.x >= 0 &&
           pos.y >= 0 &&
           pos.x < gridSize &&
           pos.y < gridSize;
}
}