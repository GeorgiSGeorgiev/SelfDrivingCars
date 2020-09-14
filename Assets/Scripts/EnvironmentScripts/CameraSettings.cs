using UnityEngine;

/// <summary>
/// Different camera modes.
/// </summary>
public enum CameraMode { Follow, FollowAndRotate };

/// <summary>
/// Class that controlls the camera movement and contains different camera settings.
/// </summary>
public class CameraSettings : MonoBehaviour {
    
    /// <summary>
    /// The target to be followed by the camera.
    /// </summary>
    public Transform Target;
    // if no target is present the camera will be set to the dummyTarget
    private Transform dummyTarget;

    // camera settings, can be editted from the Unity editor
    /// <summary>
    /// The default distance from the ground of the camera. 
    /// </summary>
    public float FollowDistance = 700.0f;
    public float MaxFollowDistance = 1000.0f;
    public float MinFollowDistance = 300.0f;

    /// <summary>
    /// Currently selected camera mode. Can be changed from the game itself.
    /// </summary>
    public CameraMode cameraMode;

    private float mouseWheel;

    /// <summary>
    /// Create dummy target if necessary.
    /// </summary>
    void Start() {
        if (Target == null) {
            // If we don't have a target assigned, create a dummy in the center of the scene.
            dummyTarget = new GameObject("Camera Target").transform;
            Target = dummyTarget;
        }
    }

    /// <summary>
    /// Update the camera's position.
    /// </summary>
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

    /// <summary>
    /// Get the mouseWheel input that can zoom in or zoom out the camera.
    /// </summary>
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

    /// <summary>
    /// Get a new camera target to be followed.
    /// </summary>
    /// <param name="newTarget">The new target.</param>
    public void GetNewTarget(Transform newTarget) {
        this.Target = newTarget;
	}
}
