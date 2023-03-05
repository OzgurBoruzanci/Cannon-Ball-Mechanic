using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{

    public static Action PositionAdjustment;
    public static Action<Vector3,float,Vector3> StartLineRenderer;
    public static Action ConstantHMax;
    public static Action NotConstantTime;
}