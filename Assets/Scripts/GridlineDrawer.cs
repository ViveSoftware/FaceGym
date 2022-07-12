using UnityEngine;
using HTC.UnityPlugin.Pointer3D;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;

public class GridlineDrawer : MonoBehaviour
{
	private const string LOG_TAG = "GridlineDrawer";

	public Pointer3DRaycaster raycaster;
	public GameObject lineSegPrefab;
	public Material chaperoneLineMaterial;

	public enum RecordState
	{
		Invalid,
		Init,
		Recording,
		Draw,
	}

	private List<Vector3> hitPoints;
	private GameObject mLineSegPrefab = null;
	private RecordState recordState = RecordState.Init;

	void OnEnable()
    {
		if (mLineSegPrefab != null)
        {
			mLineSegPrefab.SetActive(true);
		}
    }

	void OnDisable()
    {
		if (mLineSegPrefab != null)
		{
			mLineSegPrefab.SetActive(false);
		}
	}

	private bool mPainting = true;

	void Update()
	{
		var rightTriggerButton = ViveInput.GetPressEx(ControllerRole.RightHand, ControllerButton.Trigger);

		if (rightTriggerButton)
        {
			if (recordState == RecordState.Init)
            {
				Debug.Log(LOG_TAG + " [Init]");
				recordState = RecordState.Recording;

				if (hitPoints == null)
                {
					Debug.Log(LOG_TAG + " [Init] initial hitPoints list");
					hitPoints = new List<Vector3>();
				}
			}
			else if (recordState == RecordState.Recording)
            {
				Debug.Log(LOG_TAG + " [Recording] " + hitPoints.Count);
				RaycastResult r = raycaster.FirstRaycastResult();

                if (r.gameObject != null && (r.gameObject.name.Equals("RawImage (1m)")
                    || r.gameObject.name.Equals("RawImage (2m)") || r.gameObject.name.Equals("RawImage (3m)")
					|| r.gameObject.name.Equals("RawImage (5m)")))
                {
					Debug.Log(LOG_TAG + " [Recording] recording...");
					hitPoints.Add(r.worldPosition);
				}
            }
			else if (recordState == RecordState.Draw)
            {
				Debug.Log(LOG_TAG + " [Delete]");
				recordState = RecordState.Invalid;
				Destroy(mLineSegPrefab);
				mLineSegPrefab = null;
				hitPoints.Clear();
			}
		}
		else
        {
			if (recordState == RecordState.Recording)
            {
				Debug.Log(LOG_TAG + " [Draw]");
				recordState = RecordState.Draw;

                if (mLineSegPrefab == null)
                {
                    mLineSegPrefab = Instantiate(lineSegPrefab, Vector3.zero, Quaternion.identity);
                    mLineSegPrefab.GetComponent<LineRenderer>().material = chaperoneLineMaterial;
                    mLineSegPrefab.GetComponent<LineRenderer>().positionCount = hitPoints.Count;
                }

                for (int i = 0; i < hitPoints.Count; i++)
                {
					if (this.transform.name.Equals("RawImage (5m)"))
					{
						mLineSegPrefab.GetComponent<LineRenderer>().SetPosition(i, new Vector3(hitPoints[i].x, hitPoints[i].y, this.transform.position.z + 0.01f));
					}
					else if (this.transform.name.Equals("RawImage (3m)"))
					{
						mLineSegPrefab.GetComponent<LineRenderer>().SetPosition(i, new Vector3(this.transform.position.x - 0.01f, hitPoints[i].y, hitPoints[i].z));
					}
					else if (this.transform.name.Equals("RawImage (2m)"))
					{
						mLineSegPrefab.GetComponent<LineRenderer>().SetPosition(i, new Vector3(this.transform.position.x - 0.01f, hitPoints[i].y, hitPoints[i].z));
					}
					else
					{
						mLineSegPrefab.GetComponent<LineRenderer>().SetPosition(i, new Vector3(this.transform.position.x + 0.01f, hitPoints[i].y, hitPoints[i].z));
					}
				}
            }
			else if (recordState == RecordState.Invalid)
            {
				Debug.Log(LOG_TAG + " [Reset]");
				recordState = RecordState.Init;
			}
		}
	}
}
