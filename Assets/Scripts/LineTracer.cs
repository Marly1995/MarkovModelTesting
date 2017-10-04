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

    public AnimationCurve widthCurve;
    Keyframe[] defaultKeys =
    {
        new Keyframe(0.0f, 1.0f),
        new Keyframe(1.0f, 1.0f)
    };

    void Start ()
    {
        widthCurve = new AnimationCurve(defaultKeys);
        points = new List<Vector3>();
        line.startColor = Color.blue;
        line.endColor = Color.red;
        line.widthMultiplier = 0.1f;
	}

    public void SetPosition(Vector3 pos, int index)
    {
        Vector3 cPos = Camera.main.ScreenToWorldPoint(pos);
        cPos.z = 0;
        points.Add(cPos);
        line.positionCount = points.Count;
        line.SetPosition(points.Count - 1, points[points.Count - 1]);
    }

    public void ApplyHeightCurve()
    {
        Vector3[] newPoints = LineSmoother.MakeSmoothCurve(points.ToArray(), 0.1f);
        line.SetPositions(newPoints);
        line.widthMultiplier = 1.0f;
        for (int i = 1; i < points.Count - 1; i++)
        {
            float width = Mathf.Clamp(Vector3.Distance(newPoints[i-1], newPoints[i]), 0.2f, 0.4f);
            float time = (float)i / (float)points.Count;
            widthCurve.AddKey(time, 0.5f - width);
        }
        widthCurve.AddKey(0.0f, 0.0f);
        widthCurve.AddKey(1.0f, 0.0f);
        line.widthCurve = widthCurve;
    }

    public void ResetHeightCurve()
    {
        widthCurve = new AnimationCurve(defaultKeys);
        line.widthCurve = widthCurve;
        line.widthMultiplier = 0.1f;
    }

    public void ResetPositions()
    {
        points.Clear();
    }
}
