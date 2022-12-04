using CustomMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera cam;
    const uint maxVertexPerPlane = 4;
    int resolutionGrid = 10;

    int maxDivisions = 6; //Divisiones del BST

    Vector3[] frustumCornerFar = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerNear = new Vector3[maxVertexPerPlane];

    Vec3 leftMiddlePosFar;
    Vec3 leftMiddlePosNear;
    Vec3 rigthMiddlePosFar;
    Vec3 rigthMiddlePosNear;

    Vec3[] intermediatePointsFar;
    Vec3[] intermediatePointsNear;

    bool firstLoad = false;


    //SIMULA UN STRUCT
    public Room[] pointRoom;   //Es el room del punto negro, lo pongo aca xq no se puede modificar un struct de afuera xd
    public Vec3[] previousNearPos;  //Guarda la posicion anterior del middle point
    public Vec3[] previousFarPos; //Guarda la ultima posicion de "adelante"
    public Vec3[] middlePoint;

    public struct BSTCalc
    {
        public Vec3 aux1;
        public Vec3 aux2;
    }

    public Room inRoom; //Room actual del player

    private void Start()
    {
        cam = Camera.main;

        intermediatePointsFar = new Vec3[resolutionGrid];
        intermediatePointsNear = new Vec3[resolutionGrid];

        previousNearPos = new Vec3[resolutionGrid];  //Guarda la posicion anterior del middle point
        previousFarPos = new Vec3[resolutionGrid];

        pointRoom = new Room[resolutionGrid];
        middlePoint = new Vec3[resolutionGrid];

    }

    private void Update()
    {
        CalculateEndsOfFrustum();
        BinarySearch();
    }

    public void SetInRoom(Room roomIn) //Setea en que habitacion está
    {
        inRoom = roomIn;
    }

    private void CalculateEndsOfFrustum() //Se calcula y se dibujan los planos del frustrum
    {
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornerFar);  //Obtengo el Frustrum lejano
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornerNear); //Obtengo el Frustrum cercano

        for (int i = 0; i < maxVertexPerPlane; i++)
        {
            frustumCornerFar[i] = FromLocalToWorld(frustumCornerFar[i], cam.transform);
            frustumCornerNear[i] = FromLocalToWorld(frustumCornerNear[i], cam.transform);

            //ORDEN DE LOS VERTICES: 0 abajo izq, 1 arriba izq, 2 arriba der, 3 abajo der
        }

        leftMiddlePosFar = CalculateTheMiddle(frustumCornerFar[1], frustumCornerFar[0]);
        leftMiddlePosNear = CalculateTheMiddle(frustumCornerNear[1], frustumCornerNear[0]);
        rigthMiddlePosFar = CalculateTheMiddle(frustumCornerFar[2], frustumCornerFar[3]);
        rigthMiddlePosNear = CalculateTheMiddle(frustumCornerNear[2], frustumCornerNear[3]);

        intermediatePointsFar = CalculateGrid(leftMiddlePosFar, rigthMiddlePosFar);
        intermediatePointsNear = CalculateGrid(leftMiddlePosNear, rigthMiddlePosNear);

        //Setea los puntos en la mitad

        if (!firstLoad)
        {
            InitializePoints();
            firstLoad = true;
        }
    }

    private Vec3[] CalculateGrid(Vec3 leftMiddlePos, Vec3 rigthMiddlePos)
    {
        List<Vec3> gridPoints = new List<Vec3>();

        for (int i = 0; i < resolutionGrid; i++)
        {
            gridPoints.Add(Vector3.Lerp(leftMiddlePos, rigthMiddlePos, (float)i / resolutionGrid)); //Creo otra interpolacion lineal desde los puntos laterales del frustrum guardando en una lista los puntos intermedios de la grilla
        }

        return gridPoints.ToArray(); //transformo la lista a Array
    }

    Vec3 CalculateTheMiddle(Vec3 lhs, Vec3 rhs)
    {
        return new Vec3((lhs.x + rhs.x) / 2, (lhs.y + rhs.y) / 2, (lhs.z + rhs.z) / 2);
    }

    private Vector3 FromLocalToWorld(Vector3 point, Transform transformRef) //Recibe un punto y tansform de un objeto
    {
        Vector3 result = Vector3.zero;

        result = new Vector3(point.x * transformRef.localScale.x, point.y * transformRef.localScale.y, point.z * transformRef.localScale.z); //Multiplica el punto por la escala

        result = transformRef.localRotation * result; //Luego multiplica el resutado por la rotacion

        return result + transformRef.position; //El resutado le sumamos la posicion del objeto y retornamos las coordenadas en globales
    }

    public void InitializePoints()
    {
        for (int i = 0; i < resolutionGrid; i++)
        {
            middlePoint[i] = CalculateTheMiddle(intermediatePointsNear[i], intermediatePointsFar[i]);

            previousNearPos[i] = Vec3.Zero;  //Guarda la posicion anterior del middle point
            previousFarPos[i] = Vec3.Zero;
        }
    }

    public void SetPointInRoom(int point, Room roomToAdd) //Setea la habitacion en la que esta el punto
    {
        pointRoom[point] = roomToAdd;
    }

    void BinarySearch()
    {
        //InitializePoints();

        if (Input.GetKeyDown(KeyCode.S))
        {
            InitializePoints();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CalculatePointRooms();
        }
    }

    public void CalculatePointRooms()
    {
        //Si el middle esta en una habitacion conexa, tiene que ir para adelante
        //Sino, tiene que ir para atras

        for (int i = 0; i < resolutionGrid; i++)  //ESTA ENTRANDO DOS VECES ACA Y NO DEBERIA
        {
            //Tiene que preguntar si es conexo a alguna habitacion visible
            if (inRoom.associatedRooms.Contains(pointRoom[i])) //Pregunta si es conexa
            {
                previousNearPos[i] = middlePoint[i]; //Si vas para adelante, guardas tu posicion actual, que es la de atras

                if (previousFarPos[i] == Vec3.Zero) //Si es cero signfica que todavia no esta seteado, es la primer iteracion
                {
                    middlePoint[i] = CalculateTheMiddle(middlePoint[i], intermediatePointsFar[i]);
                }
                else
                {
                    middlePoint[i] = CalculateTheMiddle(middlePoint[i], previousFarPos[i]);
                }

            }
            else
            {
                previousFarPos[i] = middlePoint[i]; //Si vas para atras, guardas tu posicion actual, que es la de adelante

                if (previousNearPos[i] == Vec3.Zero)
                {
                    middlePoint[i] = CalculateTheMiddle(middlePoint[i], intermediatePointsNear[i]);
                }
                else
                {
                    middlePoint[i] = CalculateTheMiddle(middlePoint[i], previousNearPos[i]);
                }

            }

            Debug.Log(i);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < maxVertexPerPlane; i++)
        {
            Gizmos.DrawSphere(frustumCornerFar[i], .1f);
            Gizmos.DrawSphere(frustumCornerNear[i], .1f);
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < resolutionGrid; i++)
        {
            Gizmos.DrawSphere(intermediatePointsFar[i], .1f);
            Gizmos.DrawSphere(intermediatePointsNear[i], .05f);
        }

        Gizmos.color = Color.blue;

        for (int i = 0; i < resolutionGrid; i++)
        {
            Gizmos.DrawLine(intermediatePointsNear[i], intermediatePointsFar[i]);
        }

        Gizmos.color = Color.black;

        for (int i = 0; i < resolutionGrid; i++)
        {
            Gizmos.DrawSphere(middlePoint[i], .2f);
        }

        Gizmos.color = Color.white;

        for (int i = 0; i < resolutionGrid; i++)
        {
            Gizmos.DrawSphere(previousNearPos[i], .2f);
            Gizmos.DrawSphere(previousFarPos[i], .2f);
        }
    }
}
