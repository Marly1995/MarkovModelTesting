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
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.ComponentModel;

public class MousePositionRecorder : MonoBehaviour
{
    public Button StoreGestureBtn;
    public Button LearnGesturesBtn;
    public Button PredictGestureBtn;
    public Button SaveGesturesBtn;
    public Button LoadGesturesBtn;
    
    public InputField nameInputField;
    public Text text;

    List<Vector3> mousePositions;
    List<Gesture> storedGestures;
    Dictionary<string, int> gestureIndex;
    bool _isRecording;

    HiddenMarkovClassifier<MultivariateNormalDistribution, double[]> hmm;
    ITopology vector;
    
    private int index;

    public string databaseFile;
    private GestureDatabase database;

    LineTracer trail;

    void Start ()
    {
        trail = GetComponent<LineTracer>();

        database = GetComponent<GestureDatabase>();

        mousePositions = new List<Vector3>();
        storedGestures = new List<Gesture>();
        gestureIndex = new Dictionary<string, int>();

        StoreGestureBtn.onClick.AddListener(() => StoreGesture(mousePositions, nameInputField.text));
        LearnGesturesBtn.onClick.AddListener(() => LearnGesture());
        PredictGestureBtn.onClick.AddListener(() => CheckRecognized(mousePositions));
        SaveGesturesBtn.onClick.AddListener(() => SaveDatabase());
        LoadGesturesBtn.onClick.AddListener(() => LoadDatabase());
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
        //if (Input.GetMouseButtonDown(1))
        //{
        //    CheckRecognized(mousePositions);
        //}
        //if (Input.GetMouseButtonDown(2))
        //{
        //    StoreGesture(mousePositions, nameInputField.text);
        //}
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    LearnGesture();
        //}
    }

	void Update ()
    {
        HandleInput();

        if (_isRecording)
        {
            mousePositions.Add(transform.position);
            index++;
        }
	}

    public void BeginRecording()
    {
        Debug.Log("Recording Begun!");
        mousePositions.Clear();
        _isRecording = true;
        index = 0;
        trail.BeginTrail();
    }

    public void EndRecording()
    {
        Debug.Log("Recording Ended!");
        _isRecording = false;
        trail.EndTrail();
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

        List<String> classes = new List<String>();

        int states = 10;

        MultivariateNormalDistribution dist = new MultivariateNormalDistribution(3);

        hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>
            (9, new Forward(14), new MultivariateNormalDistribution(3));

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
        
        int decision = hmm.Decide(points);
        string value = string.Empty;
        foreach (KeyValuePair<string, int> item in gestureIndex)
        {
            if (item.Value == decision)
            { value = item.Key; }
        }
        text.text = value;
        nameInputField.text = value;
        Debug.Log("Did you write a: " + value + "?");
    }

    void SaveDatabase()
    {
        database.CheckDatabaseExists(databaseFile);
        database.Gestures = new BindingList<Gesture>(storedGestures);
        var stream = new FileStream(databaseFile, FileMode.Open);
        database.Save(stream);
        stream.Close();
        Debug.Log("Database Saved!");
    }

    void LoadDatabase()
    {
        database.CheckDatabaseExists(databaseFile);
        var stream = new FileStream(databaseFile, FileMode.Open);
        database.Load(stream);
        storedGestures = database.Gestures.ToList();
        for (int i = 0; i < storedGestures.Count; i++)
        {
            gestureIndex[storedGestures[i].name] = storedGestures[i].index;
        }
        stream.Close();
        Debug.Log("Database Loaded!");
    }
}

[Serializable]
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