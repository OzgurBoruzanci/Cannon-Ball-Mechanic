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

    private void OnEnable()
    {
        EventManager.PositionAdjustment += PositionAdjustment;
        EventManager.StartLineRenderer += StartLineRenderer;
    }
    private void OnDisable()
    {
        EventManager.PositionAdjustment -= PositionAdjustment;
        EventManager.StartLineRenderer -= StartLineRenderer;
    }

    void StartLineRenderer()
    {
        startPoint = ball.transform.position;
        EndPointPos();
        CalculateHMaxPoint();
        Debug.Log("ss");
    }

    void PositionAdjustment()
    {
        //startPoint = ball.transform.position;
    }

    void Start()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        startPoint = ball.transform.position;
        EndPointPos();
        CalculateHMaxPoint();

        DrawQuadraticBezierCurve(startPoint, hMaxPoint, endPoint);
    }

    void CalculateHMaxPoint()
    {
        hMaxPoint.y = Mathf.Abs(endPoint.z - startPoint.z);
        hMaxPoint.x = (endPoint.x - startPoint.x) / 2;
        hMaxPoint.z = Mathf.Abs(endPoint.z - startPoint.z) / 2;
    }

    void EndPointPos()
    {
        RaycastHit hit;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out hit))
        {
            Vector3 objectHit = hit.point;
            endPoint = objectHit;
        }
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
