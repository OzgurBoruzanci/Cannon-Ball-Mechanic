using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public bool canJump=true;
    bool collidedPlane=false;

    private void OnEnable()
    {
        EventManager.JumpControl += JumpControl;
        EventManager.CreateBall += CreateBall;
    }
    private void OnDisable()
    {
        EventManager.CreateBall -= CreateBall;
        EventManager.JumpControl -= JumpControl;
    }
    void CreateBall()
    {

    }
    void JumpControl()
    {
        canJump= false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Plane" && !canJump && transform.position.y<=0.51f)
        {
            collidedPlane= true;
            if (collidedPlane)
            {
                transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                transform.GetComponent<Rigidbody>().drag = 50;
                EventManager.CreateBall();
            }
        }
    }
}
