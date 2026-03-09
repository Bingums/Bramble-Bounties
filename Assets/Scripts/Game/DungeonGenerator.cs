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
    public float roomWidth = 21f;
    public float roomHeight = 16f;

    [Header("Seed Settings")]
    public int seed = 0;
    public bool useSeed = false;

    private bool[,] dungeonGrid;
    private Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();

    void Start()
    {
        dungeonGrid = new bool[gridSize, gridSize];

        if (useSeed)
            Random.InitState(seed);

        GenerateDungeon();
        SpawnRooms();
        ConfigureAllRooms();
    }

    // -----------------------------
    // Dungeon Generation
    // -----------------------------
    void GenerateDungeon()
    {
        Vector2Int start = new Vector2Int(gridSize / 2, gridSize / 2);
        dungeonGrid[start.x, start.y] = true;

        List<Vector2Int> frontier = new List<Vector2Int> { start };
        int createdRooms = 1;
        int maxIterations = gridSize * gridSize * 20;
        int safetyCounter = 0;

        Vector2Int[] directions = new Vector2Int[] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        while (createdRooms < roomsToCreate && safetyCounter < maxIterations)
        {
            safetyCounter++;
            if (frontier.Count == 0) break;

            Vector2Int baseRoom = frontier[Random.Range(0, frontier.Count)];
            List<Vector2Int> possibleDirs = new List<Vector2Int>();

            foreach (var dir in directions)
            {
                Vector2Int neighbor = baseRoom + dir;
                if (IsValidGridPosition(neighbor) && !dungeonGrid[neighbor.x, neighbor.y])
                {
                    possibleDirs.Add(dir);
                }
            }

            if (possibleDirs.Count == 0)
            {
                frontier.Remove(baseRoom);
                continue;
            }

            Vector2Int chosenDir = possibleDirs[Random.Range(0, possibleDirs.Count)];
            Vector2Int newPos = baseRoom + chosenDir;

            dungeonGrid[newPos.x, newPos.y] = true;
            frontier.Add(newPos);
            createdRooms++;
        }
    }

    // -----------------------------
    // Room Spawning
    // -----------------------------
    void SpawnRooms()
    {
        spawnedRooms.Clear();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!dungeonGrid[x, y]) continue;

                Vector3 position = new Vector3((x - gridSize / 2) * roomWidth, (y - gridSize / 2) * roomHeight, 0f);
                RoomData selectedRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

                GameObject roomObj = Instantiate(selectedRoom.prefab, position, Quaternion.identity);
                roomObj.name = $"Room_{x}_{y}";

                Room room = roomObj.GetComponent<Room>();
                if (room != null)
                {
                    room.DisableAllDoors(); // start clean
                    spawnedRooms[new Vector2Int(x, y)] = room;
                }
                else
                {
                    Debug.LogError("Room prefab missing Room component!");
                }
            }
        }
    }

    // -----------------------------
    // Configure Doors & Walls
    // -----------------------------
    void ConfigureAllRooms()
    {
        foreach (var kvp in spawnedRooms)
        {
            Vector2Int pos = kvp.Key;
            Room room = kvp.Value;

            // Right
            if (pos.x < gridSize - 1 && spawnedRooms.ContainsKey(pos + Vector2Int.right))
                room.EnableDoor(Room.Direction.Right);
            else
                room.DisableDoor(Room.Direction.Right);

            // Left
            if (pos.x > 0 && spawnedRooms.ContainsKey(pos + Vector2Int.left))
                room.EnableDoor(Room.Direction.Left);
            else
                room.DisableDoor(Room.Direction.Left);

            // Top
            if (pos.y < gridSize - 1 && spawnedRooms.ContainsKey(pos + Vector2Int.up))
                room.EnableDoor(Room.Direction.Top);
            else
                room.DisableDoor(Room.Direction.Top);

            // Bottom
            if (pos.y > 0 && spawnedRooms.ContainsKey(pos + Vector2Int.down))
                room.EnableDoor(Room.Direction.Bottom);
            else
                room.DisableDoor(Room.Direction.Bottom);
        }
    }

    // -----------------------------
    // Helpers
    // -----------------------------
    bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridSize && pos.y < gridSize;
    }
}