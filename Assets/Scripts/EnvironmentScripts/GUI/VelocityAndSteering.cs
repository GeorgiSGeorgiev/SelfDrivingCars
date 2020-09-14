using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that contains the Velocity UI text box and the Steering UI text box.
/// </summary>
public class VelocityAndSteering : MonoBehaviour {
    /// <summary>
    /// Car's physics. To be referenced from the Unity editor.
    /// </summary>
    public CarPhysics CarPhysics;
    /// <summary>
    /// The velocity text. To be referenced from the Unity editor.
    /// </summary>
    public Text VelocityText;
    /// <summary>
    /// The steering text. To be referenced from the Unity editor.
    /// </summary>
    public Text SteeringText;

    // Start is called before the first frame update
    void Start() {
        VelocityText.text = (CarPhysics.CurrentInput[0]*100).ToString();
        SteeringText.text = CarPhysics.CurrentInput[1].ToString();
    }

    // Updates the shown values of the velocity and the steering.
    // Update is called once per frame
    void Update() {
        VelocityText.text = (CarPhysics.CurrentInput[0] * 100).ToString();
        SteeringText.text = CarPhysics.CurrentInput[1].ToString();
    }
}