using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Button StoreGestureBtn;
    public Button LearnGesturesBtn;
    public Button PredictGestureBtn;

    public InputField nameInputField;
    public Text text;

    List<Vector3> mousePositions;
    List<Gesture> storedGestures;
    Dictionary<string, int> gestureIndex;
    bool _isRecording;

    HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> hmm;
    ITopology vector;

	void Start ()
    {
        mousePositions = new List<Vector3>();
        storedGestures = new List<Gesture>();
        gestureIndex = new Dictionary<string, int>();

        StoreGestureBtn.onClick.AddListener(() => StoreGesture(mousePositions, nameInputField.text));
        LearnGesturesBtn.onClick.AddListener(() => LearnGesture());
        PredictGestureBtn.onClick.AddListener(() => CheckRecognized(mousePositions));
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
        _isRecording = false;
    }

    void StoreGesture(List<Vector3> positions, string name)
    {
        double[][] points = new double[positions.Count][];
        for (int i = 0; i < positions.Count; i++)
        {
            points[i] = new double[3] { positions[i].x, positions[i].y, positions[i].z };
        }
        if (!gestureIndex.ContainsKey(name))
        {
            gestureIndex.Add(name, gestureIndex.Count);
        }
        Gesture gesture = new Gesture(points, name, gestureIndex[name]);
        storedGestures.Add(gesture);
        Debug.Log("Gesture Recorded as: " + name);
    }

    void LearnGesture()
    {
        double[][][] inputs = new double[storedGestures.Count][][];
        int[] outputs = new int[storedGestures.Count];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = storedGestures[i].points;
            outputs[i] = storedGestures[i].index;
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
        int decision = hmm.Decide(points);
        string value = string.Empty;
        foreach (KeyValuePair<string, int> item in gestureIndex)
        {
            if (item.Value == decision)
            { value = item.Key; }
        }
        Debug.Log("Did you write a: " + value + "?");
    }
}

public struct Gesture
{
    public double[][] points;
    public string name;
    public int index;

    public Gesture(double[][] points, string name, int index)
    {
        this.points = points;
        this.name = name;
        this.index = index;
    }
}