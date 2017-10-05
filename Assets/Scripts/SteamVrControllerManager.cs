using UnityEngine;
using System.Collections;
public class SteamVrControllerManager : MonoBehaviour
{
    public bool triggerButtonDown = false;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    MousePositionRecorder rec;

    private SteamVR_Controller.Device controller
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

        if (controller == null)
        {

            Debug.Log("Controller not initialized");

            return;

        }

        if (controller.GetPressDown(triggerButton))
        {

            rec.BeginRecording();

        }
        if (controller.GetPressUp(triggerButton))
        {

            rec.EndRecording();

        }

    }

}