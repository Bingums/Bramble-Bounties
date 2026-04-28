using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public HashSet<Room> rooms =  new HashSet<Room>();
    
    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void AddRoom(Room room)
    {
        if(rooms.Contains(room))
            rooms.Add(room);
    }
    
    public void RemoveRoom(Room room)
    {
        if(rooms.Contains(room))
            rooms.Remove(room);
    }
}
