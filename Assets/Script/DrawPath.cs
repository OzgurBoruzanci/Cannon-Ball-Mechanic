using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DrawPath : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public GameObject ball;
    Vector3 startPoint;
    Vector3 hMaxPoint;
    Vector3 endPoint;
    Camera cam;

    public bool constantHMax=true;

    private void OnEnable()
    {
        EventManager.PositionAdjustment += PositionAdjustment;
        EventManager.StartLineRenderer += StartLineRenderer;
        EventManager.ConstantHMax += ConstantHMax;
    }
    private void OnDisable()
    {
        EventManager.PositionAdjustment -= PositionAdjustment;
        EventManager.StartLineRenderer -= StartLineRenderer;
        EventManager.ConstantHMax -= ConstantHMax;
    }

    void ConstantHMax()
    {
        constantHMax= true;
    }

    void StartLineRenderer()
    {
        startPoint = ball.transform.position;
        //EndPointPos();
        DrawQuadraticBezierCurve(startPoint, CalculateHMaxPoint(startPoint, EndPointPos()), EndPointPos());
    }

    void PositionAdjustment()
    {
       // startPoint = ball.transform.position;
       ///* EndPointPos()*/;
       // CalculateHMaxPoint(startPoint, EndPointPos());
    }

    void Start()
    {
        startPoint = ball.transform.position;
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        
        EndPointPos();
        //CalculateHMaxPoint();

        DrawQuadraticBezierCurve(startPoint, CalculateHMaxPoint(startPoint,EndPointPos()), EndPointPos());
    }

    Vector3 CalculateHMaxPoint(Vector3 startPoint,Vector3 endPoint)
    {
        if (constantHMax)
        {
            hMaxPoint.y = 20;
            hMaxPoint.x = ((endPoint.x - startPoint.x) / 2)+startPoint.x;
            hMaxPoint.z = (Mathf.Abs(endPoint.z - startPoint.z) / 2)+startPoint.z;
        }
        else
        {
            hMaxPoint.y = Mathf.Abs(endPoint.z - startPoint.z);
            hMaxPoint.x = (endPoint.x - startPoint.x) / 2;
            hMaxPoint.z = Mathf.Abs(endPoint.z - startPoint.z) / 2;
        }
        return hMaxPoint;
    }

    Vector3 EndPointPos()
    {
        RaycastHit hit;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out hit))
        {
            Vector3 objectHit = hit.point;
            endPoint = objectHit;
        }
        return endPoint;
    }

    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        lineRenderer.positionCount = 200;
        float t = 0f;
        Vector3 B = /*new Vector3(0, 0, 0);*/ startPoint;
        
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            lineRenderer.SetPosition(i, B);
            t += (1 / (float)lineRenderer.positionCount);
        }
    }
}
