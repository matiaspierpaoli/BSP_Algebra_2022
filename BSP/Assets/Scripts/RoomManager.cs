using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] InputManager inputManager;
    public List<Room> rooms;

    private void OnEnable()
    {
        inputManager.ResetPointsEvent += OnResetRooms;
        inputManager.DebugPlayerEvent += OnDebugPlayer;
    }

    private void OnDisable()
    {
        inputManager.ResetPointsEvent -= OnResetRooms;
        inputManager.DebugPlayerEvent -= OnDebugPlayer;
    }

    private void Start()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].roomID = i;
            rooms[i].AddAssociatedRoom(i > 0 ? rooms[i - 1] : null);
            rooms[i].AddAssociatedRoom(i < rooms.Count - 1 ? rooms[i + 1] : null);
        }
    }

    private void OnResetRooms()
    {
        foreach (Room room in rooms)
        {
            for (int i = 0; i < player.middlePoints.Length; i++)
            {
                player.SetPointInRoom(i, null);

                if (room.CheckPointInRoom(player.middlePoints[i])) //Setea el room del punto 
                {
                    player.SetPointInRoom(i, room);
                }
            }
        }
    }

    private void OnDebugPlayer()
    {
        Debug.Log("PLAYER ROOM: " + player.inRoom);

        for (int i = 0; i < player.middlePoints.Length; i++)
        {
            Debug.Log("POINT : " + i + " " + player.pointRoom[i]);
        }
    }


    private void Update()
    {
        foreach (Room room in rooms)
        {
            if (!room.seeingRoom)
            {
                room.DisableWalls();
            }
            else
            {
                room.EnableWalls();
            }
        }

        foreach (Room room in rooms)
        {
            if (room.CheckPlayerInRoom())
            {
                player.SetInRoom(room);
            }
        }

        foreach (Room room in rooms)
        {
            for (int i = 0; i < player.middlePoints.Length; i++)
            {
                if (room.CheckPointInRoom(player.middlePoints[i])) //Setea el room del punto 
                {
                    player.SetPointInRoom(i, room);
                }
            }
        }
    }

    public void AddRoom(Room roomToAdd)
    {
        rooms.Add(roomToAdd);
        roomToAdd.roomID = rooms.Count - 1;
    }
}
