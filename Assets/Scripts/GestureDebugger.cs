using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;

public class GestureDebugger : MonoBehaviour
{
    // set values
    List<Gesture> storedGestures;
    Dictionary<string, int> gestureIndex;
    public string databaseFile;
    public GestureDatabase database;
    public GameObject[] objs;

    // test values
    List<Gesture> testStoredGestures;
    Dictionary<string, int> testGestureIndex;
    public string testDatabaseFile;
    public GestureDatabase testDatabase;
    public GameObject[] testObjs;

    [Space]
    public int dataBaseOffset;

    int index;

    // Use this for initialization
    void Start ()
    {
        LoadDatabase();
        LoadTestDatabase();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        for (int i = dataBaseOffset; i < dataBaseOffset + 10; i++)
        {
            if (index < storedGestures[i].points.Length)
            {
                objs[i-dataBaseOffset].transform.position = new Vector3((float)storedGestures[i].points[index][0], (float)storedGestures[i].points[index][1], (float)storedGestures[i].points[index][2]);
            }
        }
        for (int i = 0; i < 10; i++)
        {
            if (index < testStoredGestures[i].points.Length)
            {
                testObjs[i].transform.position = new Vector3((float)testStoredGestures[i].points[index][0], (float)testStoredGestures[i].points[index][1], (float)testStoredGestures[i].points[index][2]);
            }
        }
        index++;
	}

    public void LoadDatabase()
    {
        database.CheckDatabaseExists(databaseFile);
        var stream = new FileStream(databaseFile, FileMode.Open);
        database.Load(stream);
        storedGestures = database.Gestures.ToList();
        //for (int i = 0; i < storedGestures.Count; i++)
        //{
        //    gestureIndex[storedGestures[i].name] = storedGestures[i].index;
        //}
        stream.Close();
        Debug.Log("Database Loaded!");
    }

    public void LoadTestDatabase()
    {
        var stream = new FileStream(testDatabaseFile, FileMode.Open);
        testDatabase.Load(stream);
        testStoredGestures = testDatabase.Gestures.ToList();
        //for (int i = 0; i < storedGestures.Count; i++)
        //{
        //    gestureIndex[storedGestures[i].name] = storedGestures[i].index;
        //}
        stream.Close();
        Debug.Log("Test Database Loaded!");
    }
}
