using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;

using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;

using System;
using System.ComponentModel;
using System.Linq;

public class MousePositionRecorder : MonoBehaviour
{
    List<Vector3> mousePositions;
    List<double[][]> storedGestures;
    bool _isRecording;

    HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> hmm;
    ITopology vector;

	void Start ()
    {
        mousePositions = new List<Vector3>();
        storedGestures = new List<double[][]>();
	}
	
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginRecording();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndRecording();
        }
        if (Input.GetMouseButtonDown(1))
        {
            CheckRecognized(mousePositions);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LearnGesture();
        }
    }

	void Update ()
    {
        HandleInput();

        if (_isRecording)
        {
            mousePositions.Add(Input.mousePosition);
        }
	}

    void BeginRecording()
    {
        Debug.Log("Recording Begun!");
        mousePositions.Clear();
        _isRecording = true;
    }

    void EndRecording()
    {
        Debug.Log("Recording Ended!");
        StoreGesture(mousePositions);
        _isRecording = false;
    }

    void StoreGesture(List<Vector3> positions)
    {
        double[][] points = new double[positions.Count][];
        for (int i = 0; i < positions.Count; i++)
        {
            points[i] = new double[3] { positions[i].x, positions[i].y, positions[i].z };
        }
        storedGestures.Add(points);
        Debug.Log("Gesture Recorded!");
    }

    void LearnGesture()
    {
        double[][][] inputs = new double[storedGestures.Count][][];
        int[] outputs = new int[storedGestures.Count];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = storedGestures[i];
            outputs[i] = 0;
        }

        int states = 5;
        List<String> classes = new List<String>();

        MultivariateNormalDistribution dist = new MultivariateNormalDistribution(3);

        hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>
            (states, new Forward(states), new MultivariateNormalDistribution(3));

        var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>(hmm)
        {
            Learner = i => new BaumWelchLearning<MultivariateNormalDistribution, double[]>(hmm.Models[i])
            {
                Tolerance = 0.01,
                MaxIterations = 1,

                FittingOptions = new NormalOptions()
                {
                    Regularization = 1e-5
                }
            }
        };

        teacher.Empirical = true;
        teacher.Rejection = false;

        teacher.Learn(inputs, outputs);

        storedGestures.Clear();

        Debug.Log("Sequence Learned!");
    }

    void CheckRecognized(List<Vector3> positions)
    {
        Debug.Log("Checking sequence!");

        double[][] points = new double[positions.Count][];
        for (int i = 0; i < positions.Count; i++)
        {
            points[i] = new double[3] { positions[i].x, positions[i].y, positions[i].z };
        }

        Debug.Log(hmm.Models[0].LogLikelihood(points));
        Debug.Log(hmm.Decide(points));
    }
}
