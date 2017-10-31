using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParameters : MonoBehaviour
{
    public MousePositionRecorder gestureManager;
    public string testDatabaseLocations;

    [Space]
    public string loglocations;

    string[] gestures = 
    {
        "zero",
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine"
    };

	void Start ()
    {
        MarlonsLegitLogger.Instance.FileLocation = loglocations;
        gestureManager.LoadDatabase();
        CheckSet(3, 3, "rightPositions");
        CheckSet(6, 6, "righttransform");
        CheckSet(6, 33, "rightLeftPositions");
        CheckSet(12, 66, "rightLeftTransforms");
    }

    void CheckSet(int numValues, int valuesUsed, string name)
    {
        MarlonsLegitLogger.Instance.FileName = name;
        MarlonsLegitLogger.Instance.CreateStream();
        List<Gesture> tempGestures = new List<Gesture>();
        for (int i = 3; i < 21; i++)
        {
            float globalCorrect = 0;
            float globalWrong = 0;
            MarlonsLegitLogger.Instance.SubHeading(i.ToString());
            gestureManager.LearnGesture(numValues, i);
            for (int j = 0; j < gestures.Length; j++)
            {
                float correct = 0;
                float wrong = 0;
                tempGestures = gestureManager.LoadDatabase(testDatabaseLocations + gestures[j]);
                for (int k = 0; k < tempGestures.Count; k++)
                {
                    string str = gestureManager.CheckRecognized(tempGestures[k].points, valuesUsed);
                    MarlonsLegitLogger.Instance.Log(" [Acctual]: " + gestures[j] + "[Predicted]: " + str);
                    if (gestures[j] == str)
                    { correct++; }
                    else { wrong++; }
                }
                MarlonsLegitLogger.Instance.Log("[Correct]: " + correct + "[Wrong]: " + wrong + "[Accuracy]: " + (correct / gestures.Length));
                globalCorrect += correct;
                globalWrong += wrong;
            }
            MarlonsLegitLogger.Instance.Log("[Total Correct]: " + globalCorrect + "[Total Wrong]: " + globalWrong + "[Accuracy]: " + (globalCorrect / gestures.Length));
        }
    }
}
