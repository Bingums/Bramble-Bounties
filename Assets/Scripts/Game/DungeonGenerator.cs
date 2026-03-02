using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int gridSize = 10;
    public int roomsToCreate = 15;
    public GameObject roomPrefab;
    public float roomSpacing = 20f;

    private bool[,] dungeonGrid;

    void Start()
    {
        dungeonGrid = new bool[gridSize, gridSize];

        GenerateDungeon();
        SpawnRooms(); // IMPORTANT
    }

    void GenerateDungeon()
    {
        Vector2Int currentPos = new Vector2Int(gridSize / 2, gridSize / 2);
        dungeonGrid[currentPos.x, currentPos.y] = true;

        for (int i = 0; i < roomsToCreate; i++)
        {
            currentPos += GetRandomDirection();

            currentPos.x = Mathf.Clamp(currentPos.x, 1, gridSize - 2);
            currentPos.y = Mathf.Clamp(currentPos.y, 1, gridSize - 2);

            dungeonGrid[currentPos.x, currentPos.y] = true;
        }
    }

    Vector2Int GetRandomDirection()
    {
        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            default: return Vector2Int.right;
        }
    }

    void SpawnRooms()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (dungeonGrid[x, y])
                {
                    Vector3 position = new Vector3(x * roomSpacing, y * roomSpacing, 0);
                    GameObject roomObj = Instantiate(roomPrefab, position, Quaternion.identity);

                    Room room = roomObj.GetComponent<Room>();

                    // Check neighbors safely
                    if (IsRoomAt(x + 1, y)) room.EnableRightDoor();
                    if (IsRoomAt(x - 1, y)) room.EnableLeftDoor();
                    if (IsRoomAt(x, y + 1)) room.EnableTopDoor();
                    if (IsRoomAt(x, y - 1)) room.EnableBottomDoor();
                }
            }
        }
    }

    bool IsRoomAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridSize || y >= gridSize)
            return false;

        return dungeonGrid[x, y];
    }
}