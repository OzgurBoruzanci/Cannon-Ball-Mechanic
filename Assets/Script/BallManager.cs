using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BallManager : MonoBehaviour
{
    public GameObject cam;
    Vector3 distanceToCamera;
    public GameObject target;
    float h = 25;
    float gravity = -18;
    float positionControl;

    public bool debugPath;

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
        cam.transform.position = transform.position - distanceToCamera;
    }

    void PositionAdjustment()
    {
        CalculateH();
        TargetPos();
        Throw();
    }

    void Start()
    {
        distanceToCamera = transform.position - cam.transform.position;
        positionControl = transform.position.y;
        transform.GetComponent<Rigidbody>().useGravity = false;
        DrawPath();

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            EventManager.PositionAdjustment();
            
        }
        if (transform.position.y == positionControl)
        {
            EventManager.StartLineRenderer();
        }

        if (debugPath)
        {
            DrawPath();
        }
        
    }

    void TargetPos()
    {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out hit))
        {
            Vector3 objectHit = hit.point;
            target.transform.position = objectHit;
        }
    }

    void Throw()
    {
        Physics.gravity = Vector3.up * gravity;
        transform.GetComponent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().velocity = CalculateThrowhData().initialVelocity;
    }

    void CalculateH()
    {
        h = Mathf.Abs(target.transform.position.z - transform.position.z)/2;
    }

    ThrowhData CalculateThrowhData()
    {
        float displacementY = target.transform.position.y - transform.position.y;
        Vector3 displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new ThrowhData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    void DrawPath()
    {
        ThrowhData throwhData = CalculateThrowhData();
        Vector3 previousDrawPoint = transform.position;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * throwhData.timeToTarget;
            Vector3 displacement = throwhData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    struct ThrowhData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public ThrowhData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }
}
