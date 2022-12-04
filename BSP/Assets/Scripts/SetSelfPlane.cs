using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class SetSelfPlane : MonoBehaviour
{
    public Room room;

    public Planes plane;

    // Start is called before the first frame update
    void Start()
    {

        plane = new Planes(transform.forward, transform.position);
        room.AddMesh(this);
        room.AddPlane(this.plane);
    }
}
