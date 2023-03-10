using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameController : MonoBehaviour
{
    public enum constantValueControl { constantHMax, constantFlightTime, notConstant };
    public constantValueControl constantValue;

    public GameObject ball;
    public GameObject target;
    GameObject ballClone;
    Vector3 displacementXZ;
    Vector3 velocityY;
    Vector3 velocityXZ;

    public float height;
    public float time;
    float gravity = -18;
    float displacementY;

    bool constantHMax;
    bool constantFlightTime;

    private void OnEnable()
    {
        EventManager.StopLineRenderer += StopLineRenderer;
        EventManager.StartLineRenderer += StartLineRenderer;
        EventManager.NotConstantTime += NotConstantTime;
        EventManager.JumpControl += JumpControl;
        EventManager.CreateBall += CreateBall;
    }
    private void OnDisable()
    {
        EventManager.CreateBall -= CreateBall;
        EventManager.StopLineRenderer -= StopLineRenderer;
        EventManager.StartLineRenderer -= StartLineRenderer;
        EventManager.NotConstantTime -= NotConstantTime;
        EventManager.JumpControl -= JumpControl;
    }
    void CreateBall()
    {
        CreateCannonBall();
    }
    void JumpControl()
    {

    }
    void StopLineRenderer()
    {

    }
    void NotConstantTime()
    {

    }

    void StartLineRenderer(Vector3 startVec, float height, Vector3 endVec)
    {

    }

    void Start()
    {
        CreateCannonBall();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TargetPos();
            EventManager.StartLineRenderer(transform.position, CalculateH(time), TargetPos());
        }
        if (Input.GetMouseButtonUp(0))
        {
            Throw();
            //CreateCannonBall();
            EventManager.StopLineRenderer();
            EventManager.JumpControl();
        }

        ConstantValueChoice();
    }
    
    void CreateCannonBall()
    {
        ballClone = Instantiate(ball, transform.position, Quaternion.identity);
        ballClone.transform.GetComponent<Rigidbody>().useGravity = false;

    }

    void ConstantValueChoice()
    {
        switch (constantValue)
        {
            case constantValueControl.constantHMax:
                constantHMax = true;
                constantFlightTime = false;
                break;
            case constantValueControl.constantFlightTime:
                constantFlightTime = true;
                constantHMax = false;
                break;
            case constantValueControl.notConstant:
                constantFlightTime = false;
                constantHMax = false;
                break;
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
        ballClone.transform.GetComponent<Rigidbody>().useGravity = true;
        ballClone.transform.GetComponent<Rigidbody>().velocity = CalculateThrowhData().initialVelocity;
        Debug.Log(CalculateThrowhData().initialVelocity);
    }
    float CalcuteFlightTimeHeight(float time)
    {
        velocityY = Vector3.up * -gravity * (time / 2);
        height = (Vector3.Magnitude(velocityY) * Vector3.Magnitude(velocityY)) / (2 * -gravity);

        return height;
    }

    float CalculateH(float time)
    {
        if (constantHMax)
        {
            EventManager.NotConstantTime();
        }
        else if (constantFlightTime && !constantHMax)
        {
            height = /*-gravity * (time / 2) * (time / 2);*/ CalcuteFlightTimeHeight(time);
        }
        else if (!constantFlightTime && !constantHMax)
        {
            EventManager.NotConstantTime();
            height = Mathf.Abs(target.transform.position.z - transform.position.z);
        }
        return height;
    }

    float CalcuteFlightTime()
    {
        time = Mathf.Sqrt(-2 * CalculateH(time) / gravity) + Mathf.Sqrt(2 * (displacementY - CalculateH(time)) / gravity);

        return time;
    }

    ThrowhData CalculateThrowhData()
    {
        if (constantFlightTime)
        {
            displacementY = target.transform.position.y - transform.position.y;
            displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            height = CalculateH(time);
            velocityY = Vector3.up * -gravity * (time / 2);
            velocityXZ = displacementXZ / time;
            Debug.Log(Vector3.Magnitude((velocityY) * Vector3.Magnitude(velocityY)) / (2 * -gravity));
        }
        else
        {
            displacementY = target.transform.position.y - transform.position.y;
            displacementXZ = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            time = CalcuteFlightTime();
            velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * CalculateH(time));
            velocityXZ = displacementXZ / time;
            Debug.Log(velocityXZ + velocityY * -Mathf.Sign(gravity));
        }
        return new ThrowhData(velocityXZ + velocityY * -Mathf.Sign(gravity));
    }


    struct ThrowhData
    {
        public readonly Vector3 initialVelocity;

        public ThrowhData(Vector3 initialVelocity)
        {
            this.initialVelocity = initialVelocity;
        }

    }
}
