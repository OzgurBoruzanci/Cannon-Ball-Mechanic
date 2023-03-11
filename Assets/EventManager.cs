using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static Action<Vector3,float,Vector3> StartLineRenderer;
    public static Action StopLineRenderer;
    public static Action JumpControl;
    public static Action CreateBall;
    public static Action NotConstantTime;
}