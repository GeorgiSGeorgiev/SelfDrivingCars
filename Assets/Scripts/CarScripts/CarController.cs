using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


// AI car controller
public class CarController : MonoBehaviour {

    public uint ID { get; private set; }

    private static uint uniqueID = 0;
    private static uint StaticID { get => uniqueID++; }

    public bool KeyboardInput = false;

    public SpriteRenderer SpriteRenderer { get; private set; }

    public float MaxTimeBetweenCheckpoints = 8;
    private float sinceLastCheckpTime;

    public event Action CarExploded;

    public Agent Agent { get; set; }

    private float score;
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
        //this.KeyboardInput = SettingsMenu.PlayerInput;

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
	private void Update() {
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
        //
        // Main Entry Point
        //
		if (!this.KeyboardInput) {
            double[] sensorsOutputs = new double[sensors.Length];
            for (int i = 0; i < sensors.Length; i++) {
                sensorsOutputs[i] = sensors[i].Readings;
			}
            /*
            if (this.ID == 1)
                Debug.Log($"0.) Inputs: { sensorsOutputs[0]} { sensorsOutputs[1] } { sensorsOutputs[2] }  { sensorsOutputs[3] } { sensorsOutputs[4] }");*/
            double[] NNOutputs = this.Agent.NeuralNet.GetTheNNOutputs(sensorsOutputs);
            /*if (this.ID == 1) {
                Debug.Log($"1.) Time: { Time.deltaTime } Outputs: { NNOutputs[0]} { NNOutputs[1] } ");
                Debug.Log($"Weight count: { this.Agent.NeuralNet.TotalWeightCount } ");
            }*/
            this.Physics.SetInput(NNOutputs);
		}
	}

    private void AgentCarExplode() {
        //Debug.Log($"{this.ID} crashed");
        this.Physics.StopCar();
        this.Physics.enabled = false;
        this.enabled = false;
        this.Agent.KillAgent();
        foreach (Sensor sensor in sensors) {
            sensor.HideSprite();
		}
        this.CarExploded?.Invoke();
	}

    private void UserCarExplode() {
        this.Physics.StopCar();
        this.Physics.enabled = false;
        this.enabled = false;
        this.CarExploded?.Invoke();
    }

    public void CheckpointCaptured() {
        sinceLastCheckpTime = 0;
    }

    public void Restart() {
        this.enabled = true;
        this.sinceLastCheckpTime = 0;
        this.Physics.enabled = true;
        this.Agent?.ResurrectAgent();
        foreach (Sensor sensor in sensors) {
            sensor.ShowSprite();
            //Debug.Log("Sensor enabled");
        }
    }

    public delegate float ScoreCountingMethod(CarController car, ref int CheckpointInx);
    
    public void UpdateScore(ScoreCountingMethod countingMethod, ref int checpointInx) {
        this.Score = countingMethod(this, ref checpointInx);
        //Debug.Log($"{ checpointInx }");
    }
}

