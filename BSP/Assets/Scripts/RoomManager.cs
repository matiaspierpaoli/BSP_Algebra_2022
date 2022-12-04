using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class RoomManager : MonoBehaviourSingleton<Room>
{
    [SerializeField] Player player;

    public List<Room> rooms;

    public Room nullRoom;

    private void Start()
    {
        for (int i = 0; i < rooms.Count; i++) //Setea las ID's de las rooms
        {
            rooms[i].roomID = i;
        }

        nullRoom.roomID = rooms.Count + 1;
        nullRoom.associatedRooms.Clear(); //Es un room que esta lejos, porque no me dejaba poner "null" como room

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].AddAssociatedRoom(rooms[i]);

            if (i > 0)
            {
                rooms[i].AddAssociatedRoom(rooms[i - 1]);
            }

            if (i < rooms.Count)
            {
                rooms[i].AddAssociatedRoom(rooms[i + 1]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("PLAYER ROOM: " + player.inRoom);

            for (int i = 0; i < player.middlePoint.Length; i++)
            {
                Debug.Log("POINT : " + i + " " + player.pointRoom[i]);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (Room room in rooms)
            {
                for (int i = 0; i < player.middlePoint.Length; i++)
                {
                    player.SetPointInRoom(i, null);

                    if (room.CheckPointInRoom(player.middlePoint[i])) //Setea el room del punto 
                    {
                        player.SetPointInRoom(i, room);
                    }
                }
            }
        }

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

        if (Input.GetKeyDown(KeyCode.C))
        {
            for (int i = 0; i < player.middlePoint.Length; i++)
            {
                player.SetPointInRoom(i, nullRoom);
            }

            foreach (Room room in rooms)
            {
                for (int i = 0; i < player.middlePoint.Length; i++)
                {
                    if (room.CheckPointInRoom(player.middlePoint[i])) //Setea el room del punto 
                    {
                        player.SetPointInRoom(i, room);
                    }
                }
            }

            //player.CalculatePointRooms();
        }


        foreach (Room room in rooms)
        {
            for (int i = 0; i < player.middlePoint.Length; i++)
            {
                if (room.CheckPointInRoom(player.middlePoint[i])) //Setea el room del punto 
                {
                    player.SetPointInRoom(i, room);
                }
            }
        }
    }

    public void AddRoom(Room roomToAdd)
    {
        rooms.Add(roomToAdd);
        roomToAdd.roomID = rooms.Count;
    }
}
