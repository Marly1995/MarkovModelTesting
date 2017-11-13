using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Transform rightHand;
    [Space]
    public Transform leftHand;

    List<Vector3> rightRotations;
    List<Vector3> leftRotations;

    List<Vector3> rightPositions;
    List<Vector3> leftPositions;

    bool animating;
    int animationIndex;
    int animationStop;

    Vector3 basePosition;
	void Start ()
    {
        rightPositions = new List<Vector3>();
        rightRotations = new List<Vector3>();
        animating = false;
    }
	
	void Update ()
    {
		if (animating &&
            animationIndex < animationStop)
        {
            UpdateAnimation();
        }
	}

    void UpdateAnimation()
    {
        rightHand.position = basePosition + rightPositions[animationIndex];
    }

    public void BeginAnimation(double[][] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            rightPositions.Add(new Vector3((float)points[i][0], (float)points[i][1], (float)points[i][2]));
            rightRotations.Add(new Vector3((float)points[i][3], (float)points[i][4], (float)points[i][5]));
        }
        animating = true;
        animationIndex = 0;
        animationStop = points.Length;
        basePosition = rightHand.position;
    }
}
