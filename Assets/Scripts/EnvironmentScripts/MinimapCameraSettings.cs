using UnityEngine;

/// <summary>
/// Class that controls the minimap movement and its settings.
/// </summary>
public class MinimapCameraSettings : MonoBehaviour {
    /// <summary>
    /// The height from the ground of the camera. Can be changed from the Unity editor.
    /// </summary>
    public float FollowDistance = 700.0f;

    /// <summary>
    /// The target to be followed by the camera.
    /// </summary>
    public Transform Target;
    // if no target is present the camera will be set to the dummyTarget
    private Transform dummyTarget;

    /// <summary>
    /// The currently selected camera mode. Can be editted from the game.
    /// </summary>
    public CameraMode cameraMode;

    /// <summary>
    /// Create dummy target if necessary.
    /// </summary>
    void Start() {
        if (Target == null) {
            // If we don't have a target (assigned by the player, create a dummy in the center of the scene).
            dummyTarget = new GameObject("Camera Target").transform;
            Target = dummyTarget;
        }
    }

    /// <summary>
    /// Update the camera's position.
    /// </summary>
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

    /// <summary>
    /// Get a new camera target to be followed.
    /// </summary>
    /// <param name="newTarget">The new target.</param>
    public void GetNewTarget(Transform newTarget) {
        this.Target = newTarget;
    }
}
