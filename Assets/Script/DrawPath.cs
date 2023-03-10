using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DrawPath : MonoBehaviour
{
    private LineRenderer lineRenderer;
    Vector3 startPoint;
    Vector3 hMaxPoint;

    public bool constantHMax=true;

    private void OnEnable()
    {
        EventManager.StopLineRenderer += StopLineRenderer;
        EventManager.StartLineRenderer += StartLineRenderer;
    }
    private void OnDisable()
    {
        EventManager.StopLineRenderer -= StopLineRenderer;
        EventManager.StartLineRenderer -= StartLineRenderer;
    }
    void StopLineRenderer()
    {
        lineRenderer.enabled= false;
    }


    void StartLineRenderer(Vector3 vec,float height,Vector3 endVec)
    {
        lineRenderer.enabled= true;
        DrawQuadraticBezierCurve(vec, CalculateHMaxPoint(vec, endVec, height), endVec);
        Debug.Log("gelen " + height);
    }



    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
        point2.y = 0;
        Debug.Log("metotda " + point1);
        point1.y *= 2;

        lineRenderer.positionCount = 200;
        float t = 0f;
        Vector3 B = startPoint;
        
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            lineRenderer.SetPosition(i, B);
            t += (1 / (float)lineRenderer.positionCount);
        }
        lineRenderer.SetPosition(lineRenderer.positionCount-1, point2);
    }
}
