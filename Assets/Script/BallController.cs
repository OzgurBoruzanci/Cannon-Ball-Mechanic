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
    }
    private void OnDisable()
    {
        EventManager.JumpControl -= JumpControl;
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
                transform.GetComponent<Rigidbody>().isKinematic= true;
                EventManager.CreateBall();
            }
        }
    }
}
