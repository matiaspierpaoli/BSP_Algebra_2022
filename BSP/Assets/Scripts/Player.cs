using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    Camera cam;
    const uint maxVertexPerPlane = 4;
    int resolutionGrid = 10;

    Vector3[] frustumCornerFar = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerNear = new Vector3[maxVertexPerPlane];

    // Vectores que representan las posiciones medias a la izquierda y a la derecha del frustum en los planos lejano y cercano
    Vector3 leftMiddlePosFar;
    Vector3 leftMiddlePosNear;
    Vector3 rightMiddlePosFar;
    Vector3 rightMiddlePosNear;
   
    //Arrays de vectores que almacenan los puntos intermedios a lo largo del borde izquierdo y derecho del frustum
    Vector3[] intermediatePointsFar;
    Vector3[] intermediatePointsNear;

    //Un array de Room (habitaciones) para rastrear en qué habitación se encuentra cada punto intermedio
    public Room[] pointRoom;

    public Vector3[] previousNearPos;
    public Vector3[] previousFarPos;
    public Vector3[] middlePoints;

    //La habitación actual en la que se encuentra el jugador.
    public Room inRoom;

    private void OnEnable()
    {
        inputManager.InitializePointsEvent += OnInitializePoints;
        inputManager.CalculatePointRoomsEvent += OnCalculatePointRooms;
    }

    private void OnDisable()
    {
        inputManager.InitializePointsEvent -= OnInitializePoints;
        inputManager.CalculatePointRoomsEvent -= OnCalculatePointRooms;
    }

    private void Start()
    {
        cam = Camera.main;

        intermediatePointsFar = new Vector3[resolutionGrid];
        intermediatePointsNear = new Vector3[resolutionGrid];

        previousNearPos = new Vector3[resolutionGrid];
        previousFarPos = new Vector3[resolutionGrid];

        pointRoom = new Room[resolutionGrid];
        middlePoints = new Vector3[resolutionGrid];

        CalculateEndsOfFrustum();
        OnInitializePoints();
    }

    private void Update()
    {
        CalculateEndsOfFrustum();
    }

    public void SetInRoom(Room roomIn)
    {
        inRoom = roomIn;
    }

    /// <summary>
    /// Calcula las esquinas del frustum tanto en el plano cercano como en el lejano, luego las transforma a coordenadas de mundo. 
    /// Calcula los puntos medios a la izquierda y derecha del frustum, y luego genera los puntos intermedios entre esos puntos medios.
    /// </summary>
    private void CalculateEndsOfFrustum()
    {
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornerFar);
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornerNear);

        for (int i = 0; i < maxVertexPerPlane; i++)
        {
            frustumCornerFar[i] = cam.transform.TransformPoint(frustumCornerFar[i]);
            frustumCornerNear[i] = cam.transform.TransformPoint(frustumCornerNear[i]);
        }

        leftMiddlePosFar = CalculateTheMiddle(frustumCornerFar[1], frustumCornerFar[0]);
        leftMiddlePosNear = CalculateTheMiddle(frustumCornerNear[1], frustumCornerNear[0]);
        rightMiddlePosFar = CalculateTheMiddle(frustumCornerFar[2], frustumCornerFar[3]);
        rightMiddlePosNear = CalculateTheMiddle(frustumCornerNear[2], frustumCornerNear[3]);

        intermediatePointsFar = CalculateGrid(leftMiddlePosFar, rightMiddlePosFar);
        intermediatePointsNear = CalculateGrid(leftMiddlePosNear, rightMiddlePosNear);
    }

    /// <summary>
    /// Genera una cuadrícula de puntos interpolados entre dos posiciones dadas.
    /// </summary>
    /// <param name="leftMiddlePos">La posición en el borde izquierdo.</param>
    /// <param name="rightMiddlePos">La posición en el borde derecho.</param>
    /// <returns>Un array de <see cref="Vector3"/> que contiene los puntos interpolados entre las dos posiciones.</returns>
    private Vector3[] CalculateGrid(Vector3 leftMiddlePos, Vector3 rightMiddlePos)
    {
        List<Vector3> gridPoints = new List<Vector3>();

        for (int i = 0; i < resolutionGrid; i++)
        {
            gridPoints.Add(Vector3.Lerp(leftMiddlePos, rightMiddlePos, (float)i / resolutionGrid));
        }

        return gridPoints.ToArray();
    }

    /// <summary>
    /// Calcula el punto medio entre dos vectores
    /// </summary>
    Vector3 CalculateTheMiddle(Vector3 lhs, Vector3 rhs)
    {
        return (lhs + rhs) / 2;
    }

    /// <summary>
    /// Asigna una habitación a un punto específico
    /// </summary>
    public void SetPointInRoom(int point, Room roomToAdd)
    {
        pointRoom[point] = roomToAdd;
    }

    /// <summary>
    /// Inicializa las posiciones de los puntos intermedios (middlePoint) 
    /// y establece las posiciones previas (previousNearPos, previousFarPos) en cero
    /// </summary>
    public void OnInitializePoints()
    {
        for (int i = 0; i < resolutionGrid; i++)
        {
            middlePoints[i] = CalculateTheMiddle(intermediatePointsNear[i], intermediatePointsFar[i]);

            previousNearPos[i] = Vector3.zero;
            previousFarPos[i] = Vector3.zero;
        }
    }

    /// <summary>
    /// Verifica si un punto está en una habitación asociada con la habitación actual y
    /// ajusta la posición del punto medio basado en su posición previa o en los puntos del frustum o hacia el plano cercano
    /// </summary>
    public void OnCalculatePointRooms()
    {
        for (int i = 0; i < resolutionGrid; i++)
        {
            if (inRoom.associatedRooms.Contains(pointRoom[i]))
            {
                previousNearPos[i] = middlePoints[i];

                if (previousFarPos[i] == Vector3.zero)
                {
                    middlePoints[i] = CalculateTheMiddle(middlePoints[i], intermediatePointsFar[i]);
                }
                else
                {
                    middlePoints[i] = CalculateTheMiddle(middlePoints[i], previousFarPos[i]);
                }
            }
            else
            {
                previousFarPos[i] = middlePoints[i];

                if (previousNearPos[i] == Vector3.zero)
                {
                    middlePoints[i] = CalculateTheMiddle(middlePoints[i], intermediatePointsNear[i]);
                }
                else
                {
                    middlePoints[i] = CalculateTheMiddle(middlePoints[i], previousNearPos[i]);
                }
            }
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
            Gizmos.DrawSphere(middlePoints[i], .2f);
        }

        Gizmos.color = Color.white;

        for (int i = 0; i < resolutionGrid; i++)
        {
            Gizmos.DrawSphere(previousNearPos[i], .2f);
            Gizmos.DrawSphere(previousFarPos[i], .2f);
        }
    }
}
