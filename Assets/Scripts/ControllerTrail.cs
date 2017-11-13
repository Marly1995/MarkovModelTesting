using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTrail : MonoBehaviour {
    
    public Transform parent;
    public TrailRenderer trail;

    void Start ()
    {
        parent = transform.parent;
        trail = GetComponent<TrailRenderer>();
        StopTrailing();
    }
    
    public void StopTrailing()
    {
        transform.parent = null;
    }

    public void StartTrailing()
    {
        trail.Clear();
        transform.position = parent.position;
        transform.parent = parent;
    }
}
