using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// AI car controller
public class CarController : MonoBehaviour {
    public bool KeyboardInput = false;
    public SpriteRenderer SpriteRenderer { get; private set; }
    public float Score { get; private set; }
    private float sinceLastCheckpTime;

    /// <summary>
    /// The movement component of this car.
    /// </summary>
    public CarBehaviour Physics {
        get;
        private set;
    }

    public delegate float ScoreCountingMethod(CarController car, ref int CheckpointInx);

    public void UpdateScore(ScoreCountingMethod countingMethod, ref int chcecpointInx) {
        this.Score = countingMethod(this, ref chcecpointInx);
	}

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void CheckpointCaptured() {
        sinceLastCheckpTime = 0;
    }
}

