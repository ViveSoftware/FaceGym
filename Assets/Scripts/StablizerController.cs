using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StablizerController : MonoBehaviour
{
    public PoseStablizer poseStablizer;
    public PoseEaser poseEaser;

    private float time = 3f;

    void Update()
    {
        var buttonA = ViveInput.GetPressEx(ControllerRole.RightHand, ControllerButton.Menu);

        if (buttonA)
        {
            if (time >= 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 3f;
                poseStablizer.enabled = !poseStablizer.enabled;
                poseEaser.enabled = !poseEaser.enabled;
                ViveInput.TriggerHapticVibrationEx(ControllerRole.RightHand, 0.2f);
            }
        }
    }
}
