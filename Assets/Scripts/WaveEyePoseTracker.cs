//========= Copyright 2016-2021, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Wave.Essence.Eye;
using static HTC.UnityPlugin.Vive.VivePoseTracker;

namespace HTC.UnityPlugin.PoseTracker
{
    public class WaveEyePoseTracker : BasePoseTracker, INewPoseListener
    {
        [Serializable]
        public class UnityEventBool : UnityEvent<bool> { }

        private bool m_isValid;

        [SerializeField]
        [FormerlySerializedAs("origin")]
        private Transform m_origin;
        [SerializeField]
        [FormerlySerializedAs("onIsValidChanged")]
        private UnityEventBool m_onIsValidChanged;

        public UnityEventBool onIsValidChanged { get { return m_onIsValidChanged; } }

        protected void SetIsValid(bool value, bool forceSet = false)
        {
            if (ChangeProp.Set(ref m_isValid, value) || forceSet)
            {
                if (m_onIsValidChanged != null)
                {
                    m_onIsValidChanged.Invoke(value);
                }
            }
        }

        protected virtual void Start()
        {
            SetIsValid(false, true);
        }

        protected virtual void OnEnable()
        {
            VivePose.AddNewPosesListener(this);
        }

        protected virtual void OnDisable()
        {
            VivePose.RemoveNewPosesListener(this);
            SetIsValid(false);
        }

        public virtual void BeforeNewPoses() { }

        public virtual void OnNewPoses()
        {
#if VIU_WAVEVR_HAND_TRACKING
            var origin = default(Vector3);
            var direction = default(Vector3);
            var isValid = default(bool);

            isValid = EyeManager.Instance.GetCombindedEyeDirectionNormalized(out direction) && EyeManager.Instance.GetCombinedEyeOrigin(out origin);

            if (isValid)
            {
                var pose = default(RigidPose);

                pose = new RigidPose(origin, Quaternion.LookRotation(direction));

                if (m_origin != null && m_origin != transform.parent)
                {
                    TrackPose(pose, false);
                }
                else
                {
                    TrackPose(pose, true);
                }
            }

            SetIsValid(isValid);
#endif
        }

        public virtual void AfterNewPoses() { }
    }
}