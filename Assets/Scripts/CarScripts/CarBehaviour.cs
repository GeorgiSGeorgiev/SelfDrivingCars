using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour {
    // Custrom made settings for the game physics
    public float MaximalForwardsVelocity = 80f;
    public float MaximalBackwardsVelocity = 42f;
    public CarBehaviour[] cars;
    private const float SurfaceFriction = 20f;
    private const float Acceleration = 42f;
    private const float TurnSpeed = 110f;

    private double accelerationInput;
    private double steeringInput;

    /// <summary>
    /// Returns the accelerationInput and the steeringInput.
    /// </summary>
    public double[] CurrentInput {
        get {
            return new double[] { accelerationInput, steeringInput };
        }
    }

    /// <summary>
    /// Current velocity of the car.
    /// </summary>
    public float Velocity { get; private set; }

    /// <summary>
    /// Represents the current rotation of the car.
    /// </summary>
    public Quaternion CarRotation { get; private set; }

    private CarController mainController;

    /// <summary>
    /// Car crash event.
    /// </summary>
    public event System.Action Crash;

    // Start is called before the first frame update.
    void Start() {
        mainController = GetComponent<CarController>();
        if (cars != null) {
            foreach (var car in cars) {
                Physics2D.IgnoreCollision(car.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>(), true);
            }
        }
    }

    // Update is called once per fixed amount of time.
    void FixedUpdate() {
        if (mainController != null && mainController.KeyboardInput) {
            CheckInput();
        }
        SetInput();
        ChangeCarVelocity();
        SetSurfaceFriction();
    }

    /// <summary>
    /// Get input data from keybard (or other controller).
    /// </summary>
    private void CheckInput() {
        accelerationInput = Input.GetAxis("Vertical"); // The default keys for Vertical Axis are "Up Arrow", "Down Arrow", "W" and "S".
        steeringInput = Input.GetAxis("Horizontal");   // Default Keys for Horizontal are "Left Arrow", "Right Arrow", "A" and "D".
    }

    /// <summary>
    /// Sets the acceleration and rotation input according to the data sved in the accelerationInput and the steeringInput variables.
    /// </summary>
    private void SetInput() {
        bool canAccelerate = false;

        // accelerationInput and steeringInput are in the interval from -1 to 1.
        if (accelerationInput < -1) {
            accelerationInput = -1;
        }
        else if (accelerationInput > 1) {
            accelerationInput = 1;
        }

        if (steeringInput < -1) {
            steeringInput = -1;
        }
        else if (steeringInput > 1) {
            steeringInput = 1;
        }

        if (accelerationInput < 0) {
            canAccelerate = Velocity > accelerationInput * MaximalBackwardsVelocity; // Is Velocity in the interval ( -MaximalVelocity, 0 )?
        }
        else if (accelerationInput > 0) {
            canAccelerate = Velocity < accelerationInput * MaximalForwardsVelocity; // Is Velocity in the interval ( 0, MaximalVelocity )?
        }

        if (canAccelerate) {
            Velocity += (float)accelerationInput * Acceleration * Time.deltaTime;

            // Limit the Velocity
            if (Velocity < MaximalBackwardsVelocity * (-1)) {
                Velocity = MaximalBackwardsVelocity * (-1);
            }
            else if (Velocity > MaximalForwardsVelocity) {
                Velocity = MaximalForwardsVelocity;
            }
        }

        // Set the CarRotation.
        CarRotation = transform.rotation;
        CarRotation = CarRotation * Quaternion.AngleAxis((-1) * (float)steeringInput * TurnSpeed * Time.deltaTime, new Vector3(0, 0, 1)); // Axis is the center of the Car model.
    }

    /// <summary>
    /// Sets the accelerationInput and the steeringInput according to the inputData argument.
    /// </summary>
    public void SetInput(double[] inputData) {
        accelerationInput = inputData[0];
        steeringInput = inputData[1];
    }

    // Changes the current velocity of the car.
    private void ChangeCarVelocity() {
        Vector3 direction = new Vector3(0, 1, 0); // Vector3 is a structure
        transform.rotation = CarRotation;
        direction = CarRotation * direction;
        this.transform.position += direction * Velocity * Time.deltaTime;
    }

    // Set the surface resistance.
    private void SetSurfaceFriction() {
        if (accelerationInput == 0) {
            if (Velocity > 0) {
                Velocity -= SurfaceFriction * Time.deltaTime;
                if (Velocity < 0) { // Chack if the Velocity didn't become negative.
                    Velocity = 0;
                }
            }
            else if (Velocity < 0) {
                Velocity += SurfaceFriction * Time.deltaTime;
                if (Velocity > 0) { // Check if the Velocity didn't become positive.
                    Velocity = 0;
                }
            }
        }
    }

    // Method triggered on collision detection.
    void OnCollisionEnter2D() {
        //Debug.Log("Collision");
        Crash?.Invoke();

        // On collision, stop the car
        Velocity = 0;
    }

    /// <summary>
    /// Stops the movement of the car.
    /// </summary>
    public void Stop() {
        Velocity = 0; // Stop the car.
        CarRotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1)); // Set rotation angle to 0.
    }
}
