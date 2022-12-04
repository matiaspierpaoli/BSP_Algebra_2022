using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class Room : MonoBehaviour
{
    [SerializeField] Material green, red;

    [SerializeField] Player player;

    public List<SetSelfPlane> wallsMeshes = new List<SetSelfPlane>();

    public List<Planes> planesInRoom = new List<Planes>();

    public List<Room> associatedRooms = new List<Room>();

    public bool seeingRoom = true;

    public bool playerLooking = false;

    public int roomID;

    int pointsInsideRoom = 0;

    Vec3 offset; //Es para que si se queda en el borde, lo tome como que esta adentro

    private void Start()
    {
        offset = new Vec3(0.5f, 0.5f, 0.5f);
    }

    private void Update()
    {
        seeingRoom = CheckEnabled(); //Chequea si el jugador o alguno de los puntos del frustrum estan en el room
    }
    public void AddPlane(Planes planeToAdd)
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

        for (int i = 0; i < player.middlePoint.Length; i++)
        {
            CheckPointInRoom(player.middlePoint[i]);
        }

        return pointsInsideRoom > 0;
    }

    public bool CheckPlayerInRoom()
    {
        int checkedPlanes = 0;

        foreach (Planes plane in planesInRoom)
        {
            if (plane.GetSide(player.transform.position))
            {
                checkedPlanes++;
            }
        }

        return checkedPlanes == planesInRoom.Count;
    }

    public bool CheckPointInRoom(Vec3 pointToSearch)
    {
        int checkedPlanes = 0;

        foreach (Planes plane in planesInRoom)
        {
            if (plane.GetSide(pointToSearch))
            {
                checkedPlanes++;
            }

            if (checkedPlanes == planesInRoom.Count)
            {
                pointsInsideRoom++;
            }
        }

        return checkedPlanes == planesInRoom.Count;
    }

    public void EnableWalls()
    {
        foreach (SetSelfPlane mesh in wallsMeshes)
        {
            //mesh.GetComponent<MeshRenderer>().enabled = true;
            mesh.GetComponent<MeshRenderer>().material = green;
        }
    }

    public void DisableWalls()
    {
        foreach (SetSelfPlane mesh in wallsMeshes)
        {
            //mesh.GetComponent<MeshRenderer>().enabled = false;
            mesh.GetComponent<MeshRenderer>().material = red;
        }
    }
}

