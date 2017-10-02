using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTracer : MonoBehaviour {

    public LineRenderer line;

    public float startWidth;
    public float endWidth;

    public Color startColor;
    public Color endColor;

    List<Vector3> points;
    
	void Start ()
    {
        points = new List<Vector3>();
        line.startColor = Color.blue;
        line.endColor = Color.red;
        line.startWidth = startWidth;
        line.endWidth = endWidth;
	}

    public void SetPosition(Vector3 pos, int index)
    {
        pos.z = 0;
        points.Add(pos * 0.01f);
        line.positionCount = points.Count;
        line.SetPosition(points.Count - 1, points[points.Count - 1]);
    }

    public void ResetPositions()
    {
        points.Clear();
    }
}
