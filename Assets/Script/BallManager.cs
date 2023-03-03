using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BallManager : MonoBehaviour
{
    public GameObject cam;
    public GameObject target;
    Vector3 distanceToCamera;
    Vector3 displacementXZ;
    Vector3 positionControl;
    Vector3 velocityY;
    Vector3 velocityXZ;


    public float height;
    public float time;
    float gravity = -18;
    float displacementY;
    

    public bool debugPath;
    public bool constantHMax;
    public bool constantFlightTime;
    bool canJump=true;

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
        
    }

    void StartLineRenderer()
    {
        //if (!canJump)
        //{
        //    transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //}
        cam.transform.position = transform.position - distanceToCamera;
        
    }

    void PositionAdjustment()
    {
        CalculateH();
        TargetPos();
        Throw();
        //FlightTimeController();
    }

    void Start()
    {
        distanceToCamera = transform.position - cam.transform.position;
        positionControl = transform.position;
        transform.GetComponent<Rigidbody>().useGravity = false;
        DrawPath();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canJump)
        {
            transform.GetComponent<Rigidbody>().drag = 0;
            EventManager.PositionAdjustment();
            canJump= false;
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            EventManager.PositionAdjustment();
            canJump = false;
        }

        if (transform.position.y == positionControl.y && !canJump && transform.position != positionControl)
        {
            positionControl= transform.position;
            transform.GetComponent<Rigidbody>().drag = 50;
            //transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            transform.position = positionControl;
            EventManager.StartLineRenderer();
            //transform.GetComponent<Rigidbody>().drag = 0;
            canJump = true;
        }

        if (debugPath)
        {
            DrawPath();
        }
        if (constantHMax)
        {
            constantFlightTime = false;
        }
        if (constantFlightTime)
        {
            constantHMax = false;
        }
    }

    void TargetPos()
    {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out hit))
        {
            Vector3 objectHit = hit.point;
            target.transform.position = new Vector3(objectHit.x, 0.5f, objectHit.z);
        }
    }

    void Throw()
    {
        Physics.gravity = Vector3.up * gravity;
        transform.GetComponent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().velocity = CalculateThrowhData().initialVelocity;

    }

    float CalculateH()
    {
        if (constantHMax)
        {
            EventManager.ConstantHMax();
        }
        else
        {
            height = Mathf.Abs(target.transform.position.z - transform.position.z) / 2;
        }
        return height;
    }


    ThrowhData CalculateThrowhData()
    {
        //displacementY = target.transform.position.y - transform.position.y;
        //displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
        //time = FlightTimeController();
        //velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * CalculateH());
        //velocityXZ = displacementXZ / time;

        if (constantFlightTime)
        {
            displacementY = target.transform.position.y - transform.position.y;
            displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            height = -gravity * (time / 2) * (time / 2);
            velocityY = Vector3.up * -gravity * (time / 2);
            velocityXZ = displacementXZ / time;
        }
        else
        {
            displacementY = target.transform.position.y - transform.position.y;
            displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            time = Mathf.Sqrt(-2 * CalculateH() / gravity) + Mathf.Sqrt(2 * (displacementY - CalculateH()) / gravity);
            velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * CalculateH());
            velocityXZ = displacementXZ / time;
        }

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
