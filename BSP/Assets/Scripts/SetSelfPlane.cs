using UnityEngine;

public class SetSelfPlane : MonoBehaviour
{
    public Room room;
    public Plane plane;

    private void Start()
    {
        plane = new Plane(transform.forward, transform.position);
        room.AddMesh(this);
        room.AddPlane(plane);
    }
}
