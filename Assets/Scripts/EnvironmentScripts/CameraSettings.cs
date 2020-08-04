using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraSettings : MonoBehaviour {
    public enum CameraMode { Follow, FollowAndRotate };

    public Transform Target;
    private Transform dummyTarget;

    public float FollowDistance = 700.0f;
    public float MaxFollowDistance = 1000.0f;
    public float MinFollowDistance = 300.0f;
    public CameraMode cameraMode;

    private float mouseWheel;

    void Start() {
        if (Target == null) {
            // If we don't have a target (assigned by the player, create a dummy in the center of the scene).
            dummyTarget = new GameObject("Camera Target").transform;
            Target = dummyTarget;
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        GetPlayerInput();
        // Check if we have a valid target
        if (Target != null) {
            transform.position = Target.position + Target.TransformDirection(new Vector3(0, 0, -FollowDistance));
            transform.LookAt(Target);

            if (cameraMode == CameraMode.FollowAndRotate) {
                transform.eulerAngles = Target.eulerAngles;
            }
        }
    }


    void GetPlayerInput() {
        // Check Mouse Wheel Input prior to Shift Key so we can apply multiplier on Shift for Scrolling
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        // Check MouseWheel to Zoom in-out
        if (mouseWheel < -0.01f || mouseWheel > 0.01f) {
            FollowDistance -= mouseWheel * 170.0f;
            // Limit FollowDistance between min & max values.
            FollowDistance = Mathf.Clamp(FollowDistance, MinFollowDistance, MaxFollowDistance);
        }
    }

    public void GetNewTarget(Transform newTarget) {
        this.Target = newTarget;
	}
}
