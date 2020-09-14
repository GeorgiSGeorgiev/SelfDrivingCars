using System;
using UnityEngine;

// AI car controller
/// <summary>
/// Contains the CarPhysics and car's Agent. This is the main controller of the Car.
/// </summary>
public class CarController : MonoBehaviour {
    /// <summary>
    /// Controller ID.
    /// </summary>
    public uint ID { get; private set; }

    // used to calculate the ID of this Controller.
    private static uint uniqueID = 0;
    private static uint StaticID { get => uniqueID++; }

    /// <summary>
    /// Indicates if the user controlls the car or not.
    /// </summary>
    public bool KeyboardInput = false;

    /// <summary>
    /// The sprite (visual game model) of the car. To be referenced from the Unity editor.
    /// Allows different model changes from the code.
    /// </summary>
    public SpriteRenderer SpriteRenderer { get; private set; }

    /// <summary>
    /// Time until the car is disabled.
    /// </summary>
    public float MaxTimeBetweenCheckpoints = 8;
    private float sinceLastCheckpTime;

    /// <summary>
    /// Event triggered on car destruction.
    /// </summary>
    public event Action CarExploded;

    /// <summary>
    /// The car's agent which contains the NeuralNet and the Genotype.
    /// </summary>
    public Agent Agent { get; set; }

    private float score;
    /// <summary>
    /// The score of the current controller.
    /// To calculate the score Genotype's Evaluation is used.
    /// </summary>
    public float Score {
        get {
            if (this.Agent != null) {
                return Agent.Genotype.Evaluation;
            }
            return this.score;
        }
        set {
            if (this.Agent != null) {
                Agent.Genotype.Evaluation = value;
            }
            else {
                this.score = value;
            }
        }
    }

    private Sensor[] sensors;

    /// <summary>
    /// Physics of the car.
    /// </summary>
    public CarPhysics Physics {
        get;
        private set;
    }

	public void Awake() {
        this.SpriteRenderer = GetComponent<SpriteRenderer>();
        this.Physics = GetComponent<CarPhysics>();
        this.sensors = GetComponentsInChildren<Sensor>();

        this.ID = CarController.StaticID;
        // security check
        if (CarController.uniqueID == uint.MaxValue) {
            CarController.uniqueID = 0;
		}
        this.name = $"Agent { this.ID }";
        if (!KeyboardInput) {
            Physics.Crash += AgentCarExplode;
        }
        else {
            Physics.Crash += UserCarExplode;
        }
    }

	// Update is called once per frame
	private void Update() {
        this.sinceLastCheckpTime += Time.deltaTime;
    }

    /// <summary>
    /// Get sensor readings, do the NN calculations and set car's velocity and steering.
    /// </summary>
	private void FixedUpdate() {
        if (sinceLastCheckpTime > this.MaxTimeBetweenCheckpoints) {
            // timed out
            if (!KeyboardInput) {
                this.AgentCarExplode();
            }
            else {
                this.UserCarExplode();
            }
            return;
		}

        //
        // Main Entry Point
        // Get sensor readings, do the NN calculations and set car's velocity and steering
        //
		if (!this.KeyboardInput) {
            // get sensor readings
            double[] sensorsOutputs = new double[sensors.Length];
            for (int i = 0; i < sensors.Length; i++) {
                sensorsOutputs[i] = sensors[i].Readings;
			}

            // get the neural network outputs
            double[] NNOutputs = this.Agent.NeuralNet.GetTheNNOutputs(sensorsOutputs);
            
            // set the car velocity and steering
            this.Physics.SetInput(NNOutputs);
		}
	}

    private void AgentCarExplode() {
        this.Physics.StopCar();
        this.Physics.enabled = false;
        this.enabled = false;
        this.Agent.KillAgent();
        this.CarExploded?.Invoke();
	}

    private void UserCarExplode() {
        this.Physics.StopCar();
        this.Physics.enabled = false;
        this.enabled = false;
        this.CarExploded?.Invoke();
    }

    /// <summary>
    /// Reset the <c>sinceLastCheckpTime</c> variable.
    /// </summary>
    public void CheckpointCaptured() {
        sinceLastCheckpTime = 0;
    }

    /// <summary>
    /// Reset the CarController.
    /// </summary>
    public void Restart() {
        this.enabled = true;
        this.sinceLastCheckpTime = 0;
        this.Physics.enabled = true;
        this.Agent?.ResurrectAgent();
    }

    /// <summary>
    /// Public delegate to the method that counts the Score of the car.
    /// </summary>
    /// <param name="car">The targeted car.</param>
    /// <param name="CheckpointInx">The index of the last captured checkpoint. This value can be changed from the method according to the capture distance of the next checkpoint.</param>
    /// <returns></returns>
    public delegate float ScoreCountingMethod(CarController car, ref int CheckpointInx);
    
    /// <summary>
    /// Calculate the new score.
    /// </summary>
    /// <param name="countingMethod">A method which calculates the new score.</param>
    /// <param name="checpointInx">Currently captured checkpoint index.</param>
    public void UpdateScore(ScoreCountingMethod countingMethod, ref int checpointInx) {
        this.Score = countingMethod(this, ref checpointInx);
    }
}

