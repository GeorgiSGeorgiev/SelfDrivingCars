using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraSettings : MonoBehaviour {
    public enum CameraMode { Follow, FollowAndRotate };

    public float FollowDistance = 700.0f;
    public Transform Target;
    private Transform dummyTarget;

    public CameraMode cameraMode;

    void Start() {
        if (Target == null) {
            // If we don't have a target (assigned by the player, create a dummy in the center of the scene).
            dummyTarget = new GameObject("Camera Target").transform;
            Target = dummyTarget;
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        // Check if we have a valid target
        if (Target != null) {
            transform.position = Target.position + Target.TransformDirection(new Vector3(0, 0, -FollowDistance));
            transform.LookAt(Target);

            if (cameraMode == CameraMode.FollowAndRotate) {
                transform.eulerAngles = Target.eulerAngles;
            }
        }
    }

    public void GetNewTarget(Transform newTarget) {
        this.Target = newTarget;
    }
}
