using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackController : MonoBehaviour {
    public static TrackController TC;

    public CameraSettings MainCamera;
    public VelocityAndSteering stats;

    private int generationCounter = 0;
    public int GenerationCounter { 
        get => generationCounter;
        private set => generationCounter = value;
    }

    public Text GenerationTextBox; 

    [SerializeField]
    private bool AutomaticRestart = true;
    private bool RestartButtonClicked = false;

    private AllCheckpoints checkpoints;

    public float TrackLength { get => checkpoints.TrackLength; }

    /// <summary>
    /// Car that we clone. It is used as a user controlled car if the KeyboardInput boolean is set to true in the game.
    /// </summary>
    public CarController PrototypeCarModel;

    private Vector3 startingPos;
    private Quaternion startingRot;
    public List<Car> cars { get; private set; } = new List<Car>();

    private CarController winningCar;

    public CarController WinningCar {
        get => winningCar;
        private set {
            if (winningCar != value) {
                if (value != null) {
                    value.SpriteRenderer.color = Color.green;
				}
                if (WinningCar != null) {
                    WinningCar.SpriteRenderer.color = Color.white;
				}

                CarController tempCar = winningCar;
                winningCar = value;
                WinningCarHasChanged?.Invoke(WinningCar);
            }
		}
	}

    /// <summary>
    /// Event that means we have a new winning car.
    /// </summary>
    public event Action<CarController> WinningCarHasChanged;

    private CarController secondPosCar;

    public CarController SecondPosCar {
        get => secondPosCar;
        private set {
            if (secondPosCar != value) {
                if (value != null) {
                    value.SpriteRenderer.color = Color.yellow;
				}
                if (secondPosCar != null && secondPosCar != WinningCar) {
                    secondPosCar.SpriteRenderer.color = Color.white;
				}
                this.secondPosCar = value;

                SecondPosCarHasChanged?.Invoke(SecondPosCar);

			}
		}
	}

    /// <summary>
    /// Event that means we have a new second posistion car.
    /// </summary>
    public event Action<CarController> SecondPosCarHasChanged;
    

	public void Awake() {
        if (TrackController.TC != null) {
            throw new Exception("The TrackControllerInstance was already created.");
        }

        TrackController.TC = this;

        this.GenerationCounter = 0;
        this.GenerationTextBox.text = this.generationCounter.ToString();

        this.checkpoints = new AllCheckpoints(GetComponentsInChildren<Checkpoint>());

        this.startingPos = PrototypeCarModel.transform.position;
        this.startingRot = PrototypeCarModel.transform.rotation;
        PrototypeCarModel.gameObject.SetActive(false);
	}

	public void Start() {
		foreach (Checkpoint checkp in this.checkpoints) {
            checkp.IsVisible = false;
		}
    }

    public void FixedUpdate() {
        //Debug.Log("NONON");
        for (int i = 0; i < cars.Count; i++) {
            if (cars[i].CarController.enabled) {
                cars[i].CarController.UpdateScore(checkpoints.GetCompletionScore, ref cars[i].CheckpointCount);
                // update best car
                if (WinningCar == null || cars[i].CarController.Score > WinningCar.Score) {
                    WinningCar = cars[i].CarController;
                    //Debug.Log($"New best car, Score: { winningCar.Score}");
                }
                // update second-position car
                if ((SecondPosCar == null || cars[i].CarController.Score > SecondPosCar.Score) && cars[i].CarController != WinningCar) {
                    SecondPosCar = cars[i].CarController;
                }
            }
            // car crashed, change its sprite
            else if (! (cars[i].CarController == WinningCar || cars[i].CarController == SecondPosCar)) {
                cars[i].UpdateColor(Color.red);
                //Debug.Log("NONON");
            }
        }
    }

    public void UpdateCarCount(int carCount) {
        if (carCount <= 0) {
            throw new ArgumentException("carCount must not be less that or equal to zero");
        }
        if (carCount == cars.Count) { return; }
        // remove cars
        if (carCount < cars.Count) {
            int removeCount = cars.Count - carCount;
            for (; removeCount > 0; removeCount--) {
                Car car = cars[cars.Count - 1];
                cars.RemoveAt(cars.Count - 1);
                Destroy(car.CarController.gameObject);
			}
            return;
		}
        if (carCount > cars.Count) {
            int addCount = carCount - cars.Count;
            for (; addCount > 0; addCount--) {
                GameObject newCar = Instantiate(PrototypeCarModel.gameObject);
                newCar.transform.position = startingPos;
                newCar.transform.rotation = startingRot;
                CarController carController = newCar.GetComponent<CarController>();
                // add the new car controller to the game logic
                cars.Add(new Car(carController));
                newCar.SetActive(true);
			}
            return;
		}
    }
    
    /// <summary>
    /// Restarts the whole simulation.
    /// </summary>
	public void Restart() {
        
        this.generationCounter++;
        this.GenerationTextBox.text = this.generationCounter.ToString();
		if (this.AutomaticRestart) {
            InstantRestart();
            return;
		}
        if (this.RestartButtonClicked) {
            InstantRestart();
            this.RestartButtonClicked = false;
            return;
		}
	}

    private void InstantRestart() {
        foreach (Car car in cars) {
            car.CarController.transform.position = startingPos;
            car.CarController.transform.rotation = startingRot;
            car.CarController.Restart();
            car.UpdateColor(Color.white);
            car.CheckpointCount = 1;
        }

        WinningCar = null;
        SecondPosCar = null;
    }

    public IEnumerator<CarController> GetCarEnumerator() {
        for (int i = 0; i < cars.Count; i++)
            yield return cars[i].CarController;
    }
}
