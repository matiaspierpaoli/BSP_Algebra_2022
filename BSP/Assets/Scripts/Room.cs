using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] Material green, red;
    [SerializeField] Player player;

    public List<SetSelfPlane> wallsMeshes = new List<SetSelfPlane>();
    public List<Plane> planesInRoom = new List<Plane>();
    public List<Room> associatedRooms = new List<Room>();

    public bool seeingRoom = true;
    public bool playerLooking = false;
    public int roomID;
    int pointsInsideRoom = 0;

    private void Start()
    {
        associatedRooms.Add(this); // Associate room with itself
    }

    private void Update()
    {
        seeingRoom = CheckEnabled();
    }

    public void AddPlane(Plane planeToAdd)
    {
        planesInRoom.Add(planeToAdd);
    }

    public void AddMesh(SetSelfPlane meshToAdd)
    {
        wallsMeshes.Add(meshToAdd);
    }

    public void AddAssociatedRoom(Room roomToAdd)
    {
        associatedRooms.Add(roomToAdd);
    }

    public bool CheckEnabled()
    {
        pointsInsideRoom = 0;
        CheckPointInRoom(player.transform.position);

        foreach (var point in player.middlePoints)
        {
            CheckPointInRoom(point);
        }

        return pointsInsideRoom > 0;
    }

    public bool CheckPlayerInRoom()
    {
        int checkedPlanes = 0;
        foreach (var plane in planesInRoom)
        {
            if (plane.GetSide(player.transform.position))
            {
                checkedPlanes++;
            }
        }
        return checkedPlanes == planesInRoom.Count;
    }

    public bool CheckPointInRoom(Vector3 pointToSearch)
    {
        int checkedPlanes = 0;
        foreach (var plane in planesInRoom)
        {
            if (plane.GetSide(pointToSearch))
            {
                checkedPlanes++;
            }
        }
        if (checkedPlanes == planesInRoom.Count)
        {
            pointsInsideRoom++;
        }
        return checkedPlanes == planesInRoom.Count;
    }

    public void EnableWalls()
    {
        foreach (var mesh in wallsMeshes)
        {
            mesh.GetComponent<MeshRenderer>().material = green;
        }
    }

    public void DisableWalls()
    {
        foreach (var mesh in wallsMeshes)
        {
            mesh.GetComponent<MeshRenderer>().material = red;
        }
    }
}
