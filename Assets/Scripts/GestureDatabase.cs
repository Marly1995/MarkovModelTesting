using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;

public class GestureDatabase : MonoBehaviour
{

    public BindingList<Gesture> Gestures;
    
	void Start ()
    {
        Gestures = new BindingList<Gesture>();
    }

    public void Save(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(BindingList<Gesture>));
        serializer.Serialize(stream, Gestures);
    }

    public void Load(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(BindingList<Gesture>));
        var samples = (BindingList<Gesture>)serializer.Deserialize(stream);

        Gestures.Clear();
        foreach (Gesture sample in samples)
        {
            Gestures.Add(sample);
        }
    }

    public void CheckDatabaseExists(string name)
    {
        var stream = new FileStream(name, FileMode.OpenOrCreate);
        stream.Close();
    }
}
