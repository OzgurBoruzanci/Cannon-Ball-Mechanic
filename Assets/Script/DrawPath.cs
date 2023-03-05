using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DrawPath : MonoBehaviour
{
    private LineRenderer lineRenderer;
    Vector3 startPoint;
    Vector3 hMaxPoint;
    bool lineRendererBool=false;
    bool notConstantTime = false;

    public bool constantHMax=true;

    private void OnEnable()
    {
        EventManager.PositionAdjustment += PositionAdjustment;
        EventManager.StartLineRenderer += StartLineRenderer;
        EventManager.ConstantHMax += ConstantHMax;
        EventManager.NotConstantTime += NotConstantTime;
    }
    private void OnDisable()
    {
        EventManager.PositionAdjustment -= PositionAdjustment;
        EventManager.StartLineRenderer -= StartLineRenderer;
        EventManager.ConstantHMax -= ConstantHMax;
        EventManager.NotConstantTime -= NotConstantTime;
    }
    void NotConstantTime()
    {
        notConstantTime=true;
    }

    void ConstantHMax()
    {
        constantHMax= true;
    }

    void StartLineRenderer(Vector3 vec,float height,Vector3 endVec)
    {
        if (!Input.GetMouseButtonDown(0))
        {
            DrawQuadraticBezierCurve(vec, CalculateHMaxPoint(vec, endVec, height), endVec);
            lineRendererBool = true;
        }
    }

    void PositionAdjustment()
    {
        lineRendererBool= false;
      
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        
    }

    Vector3 CalculateHMaxPoint(Vector3 startPoint,Vector3 endPoint,float height)
    {
        hMaxPoint.y = height;
        hMaxPoint.x = ((endPoint.x - startPoint.x) / 2) + startPoint.x;
        hMaxPoint.z = (Mathf.Abs(endPoint.z - startPoint.z) / 2) + startPoint.z;
        return hMaxPoint;
    }

    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        if (notConstantTime)
        {
            point1.y *= 2;
        }
        lineRenderer.positionCount = 200;
        float t = 0f;
        Vector3 B = startPoint;
        
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            lineRenderer.SetPosition(i, B);
            t += (1 / (float)lineRenderer.positionCount);
        }
    }
}
