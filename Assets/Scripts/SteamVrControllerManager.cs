using UnityEngine;
using System.Collections;
public class SteamVrControllerManager : MonoBehaviour
{
    private Valve.VR.EVRButtonId downPadButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    MousePositionRecorder rec;

    private SteamVR_Controller.Device Controller
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

    private SteamVR_TrackedObject trackedObj;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        rec = GetComponent<MousePositionRecorder>();
    }

    void Update()
    {
        if (Controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if (Controller.GetPressDown(triggerButton))
        {
            rec.BeginRecording();
        }
        if (Controller.GetPressUp(triggerButton))
        {
            rec.EndRecording();
        }
        if (Controller.GetPressDown(downPadButton))
        {
            rec.StoreGesture();
        }
    }
}