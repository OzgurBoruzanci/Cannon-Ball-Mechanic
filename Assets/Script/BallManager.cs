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

    }
    void ConstantHMax()
    {
        
    }

    void StartLineRenderer(Vector3 startVec,float height,Vector3 endVec)
    {
        
    }

    void PositionAdjustment()
    {
        TargetPos();
        Throw();
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
        if (transform.position.y<0.8f)
        {
            EventManager.StartLineRenderer(transform.position, CalculateH(), TargetPos());
        }
        CalculateH();
        if (Input.GetMouseButtonDown(0) && canJump)
        {
            transform.GetComponent<Rigidbody>().drag = 0;
            EventManager.PositionAdjustment();
            canJump= false;
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            canJump = false;
        }

        if (transform.position.y == positionControl.y && !canJump && transform.position != positionControl)
        {
            positionControl= transform.position;
            transform.GetComponent<Rigidbody>().drag = 50;
            transform.position = positionControl;
            canJump = true;
            cam.transform.position = transform.position - distanceToCamera;
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

    Vector3 TargetPos()
    {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out hit))
        {
            Vector3 objectHit = hit.point;
            target.transform.position = new Vector3(objectHit.x, 0.5f, objectHit.z);
        }
        return target.transform.position;
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
            EventManager.NotConstantTime();
        }
        else if (constantFlightTime && !constantHMax)
        {
            height = -gravity * (time / 2) * (time / 2);
        }
        else if(!constantFlightTime && !constantHMax)
        {
            EventManager.NotConstantTime();
            height = Mathf.Abs(target.transform.position.z - transform.position.z);
        }
        return height;
    }


    ThrowhData CalculateThrowhData()
    {
        if (constantFlightTime)
        {
            displacementY = target.transform.position.y - transform.position.y;
            displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            height = CalculateH();
            velocityY = Vector3.up * -gravity * (time / 2);
            velocityXZ = displacementXZ / time;
        }
        else
        {
            displacementY = target.transform.position.y - transform.position.y;
            displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            time = Mathf.Sqrt(-2 * CalculateH() / gravity) + Mathf.Sqrt(2 * (displacementY - CalculateH()) / gravity);
            velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * CalculateH()); /*Vector3.up * -gravity * (time / 2);*/
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
