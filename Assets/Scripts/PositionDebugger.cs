using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionDebugger : MonoBehaviour
{
    public Text local;
    public Text world;
	
	// Update is called once per frame
	void Update ()
    {
        local.text = "Local\n" + "X: " + transform.position.x.ToString("F3") + "\n" +
                     "Y: " + transform.position.y.ToString("F3") + "\n" +
                     "Z: " + transform.position.z.ToString("F3") + "\n";

        world.text = "World\n" + "X: " + transform.localPosition.x.ToString("F3") + "\n" +
                     "Y: " + transform.localPosition.y.ToString("F3") + "\n" +
                     "Z: " + transform.localPosition.z.ToString("F3") + "\n";
    }
}
