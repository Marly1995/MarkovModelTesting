using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTracer : MonoBehaviour {
    
    public ControllerTrail controller;

    public void BeginTrail()
    {
        controller.StartTrailing();
    }

    public void EndTrail()
    {
        controller.StopTrailing();
    }
}
