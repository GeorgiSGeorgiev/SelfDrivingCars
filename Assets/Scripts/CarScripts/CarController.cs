using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// AI car controller
public class CarController : MonoBehaviour {

    public uint ID { get; private set; }

    private static uint uniqueID = 0;
    private static uint StaticID { get => uniqueID++; }

    public bool KeyboardInput = false;
    public SpriteRenderer SpriteRenderer { get; private set; }

    public float MaxTimeBetweenCheckpoints = 6;
    private float sinceLastCheckpTime;

    public Agent Agent { get; set; }

    public float Score {
        get => Agent.Genotype.Evaluation;
        set => Agent.Genotype.Evaluation = value;
    }

    private Sensor[] sensors;

    /// <summary>
    /// Physics of the car.
    /// </summary>
    public CarPhysics Physics {
        get;
        private set;
    }

	private void Awake() {
        this.SpriteRenderer = GetComponent<SpriteRenderer>();
        this.Physics = GetComponent<CarPhysics>();
        this.sensors = GetComponentsInChildren<Sensor>();
	}

    // Start is called before the first frame update
    void Start() {
        this.ID = CarController.StaticID;
        this.name = $"Agent { this.ID }";
        if (!KeyboardInput) {
            Physics.Crash += AgentCarExplode;
        }
        else {
            Physics.Crash += UserCarExplode;
        }
    }

    // Update is called once per frame
    void Update() {
        this.sinceLastCheckpTime += Time.deltaTime;
    }

	private void FixedUpdate() {
        if (sinceLastCheckpTime > this.MaxTimeBetweenCheckpoints) {
            if (!KeyboardInput) {
                this.AgentCarExplode();
            }
            else {
                this.UserCarExplode();
            }
            return;
		}
		if (!this.KeyboardInput) {
            double[] sensorsOutputs = new double[sensors.Length];
            for (int i = 0; i < sensors.Length; i++) {
                sensorsOutputs[i] = sensors[i].Readings;
			}
            double[] NNOutputs = Agent.NeuralNet.GetTheNNOutputs(sensorsOutputs);
            this.Physics.SetInput(NNOutputs);
		}
	}

    private void AgentCarExplode() {
        this.Physics.StopCar();
        this.Physics.enabled = false;
        this.enabled = false;
        this.Agent.KillAgent();
        foreach (Sensor sensor in sensors) {
            sensor.HideSprite();
		}
	}

    private void UserCarExplode() {
        this.Physics.StopCar();
        this.Physics.enabled = false;
        this.enabled = false;
    }

    public void CheckpointCaptured() {
        sinceLastCheckpTime = 0;
    }

    public void Restart() {
        this.sinceLastCheckpTime = 0;
        this.Physics.enabled = true;
        this.Agent.ResurrectAgent();
        this.enabled = true;
        foreach (Sensor sensor in sensors) {
            sensor.ShowSprite();
        }
    }

    public delegate float ScoreCountingMethod(CarController car, ref int CheckpointInx);

    public void UpdateScore(ScoreCountingMethod countingMethod, ref int chcecpointInx) {
        this.Score = countingMethod(this, ref chcecpointInx);
    }
}

