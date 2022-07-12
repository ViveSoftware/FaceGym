using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using ViveSR.anipal.Eye;

public class Player : MonoBehaviour
{
    public Transform objects;
    public Transform head;
    public Transform vrOrigin;
    public DartBoard[] dartboards;
    public GameObject dartboardRoot;
    public GameObject gridlineRoot;
    public GameObject reticle;
    public GridlineDrawer[] gridlines;

    private ViveRoleProperty hmdRole = ViveRoleProperty.New(DeviceRole.Hmd);
    private bool isInitialized = false;
    private float time = 1f;
    private float reticleTime = 3f;

    private void Update()
    {
        var hmd = VRModule.GetCurrentDeviceState(hmdRole.GetDeviceIndex());

        if (hmd.isPoseValid && !isInitialized)
        {
            isInitialized = true;
            objects.position = new Vector3(objects.position.x, hmd.position.y, objects.position.z);

            for (int i = 0; i < dartboards.Length; i++)
            {
                dartboards[i].Focus(Vector3.zero);
            }
        }

        var systemButtonPressed = ViveInput.GetPressEx(ControllerRole.LeftHand, ControllerButton.System);

        if (systemButtonPressed && dartboardRoot.activeSelf)
        {
            if (time >= 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 1f;
                objects.transform.position = new Vector3(objects.transform.position.x, hmd.position.y, objects.transform.position.z);

                for (int i = 0; i < dartboards.Length; i++)
                {
                    dartboards[i].Focus(Vector3.zero);
                }

                ViveInput.TriggerHapticVibrationEx(ControllerRole.LeftHand, 0.2f);
            }
        }

        var rightGripButtonPressed = ViveInput.GetPressEx(ControllerRole.RightHand, ControllerButton.Grip);
        if (rightGripButtonPressed)
        {
            if (time >= 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 1f;

                if (dartboardRoot.activeSelf)
                {
                    gridlineRoot.SetActive(true);
                    dartboardRoot.SetActive(false);
                }
                else
                {
                    gridlineRoot.SetActive(false);
                    dartboardRoot.SetActive(true);
                }

                ViveInput.TriggerHapticVibrationEx(ControllerRole.RightHand, 0.2f);
            }
        }

        var buttonA = ViveInput.GetPressEx(ControllerRole.RightHand, ControllerButton.AKey);

        if (buttonA)
        {
            if (reticleTime >= 0)
            {
                reticleTime -= Time.deltaTime;
            }
            else
            {
                reticleTime = 3f;
                reticle.SetActive(!reticle.activeSelf);

                ViveInput.TriggerHapticVibrationEx(ControllerRole.RightHand, 0.2f);
            }
        }
    }
}
