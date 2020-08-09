using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour {
    public static int TrackControllerCount = 0;
    public static TrackController TCInstance;

    [SerializeField]
    private Sprite WinningCarSprite;

    [SerializeField]
    private Sprite SecondPosCarSprite;

    [SerializeField]
    private Sprite NormalCarSprite;

    [SerializeField]
    private Sprite DeadCarSprite;

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
    private List<Car> cars = new List<Car>();

    private CarController winningCar;

    public CarController WinningCar {
        get => winningCar;
        private set {
            if (winningCar != value) {
                if (value != null) {
                    value.SpriteRenderer.sprite = WinningCarSprite;
				}
                if (WinningCar != null) {
                    WinningCar.SpriteRenderer.sprite = NormalCarSprite;
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
            if (SecondPosCar != value) {
                if (value != null) {
                    value.SpriteRenderer.sprite = SecondPosCarSprite;
				}
                if (SecondPosCar != null && SecondPosCar != WinningCar) {
                    SecondPosCar.SpriteRenderer.sprite = NormalCarSprite;
				}
                this.SecondPosCar = value;

                SecondPosCarHasChanged?.Invoke(SecondPosCar);
			}
		}
	}

    /// <summary>
    /// Event that means we have a new second posistion car.
    /// </summary>
    public event Action<CarController> SecondPosCarHasChanged;

	private void Awake() {
		if (TrackController.TCInstance != null) { return; }

        TrackController.TCInstance = this;

        this.checkpoints = new AllCheckpoints(GetComponentsInChildren<Checkpoint>());

        this.startingPos = PrototypeCarModel.transform.position;
        this.startingRot = PrototypeCarModel.transform.rotation;
        PrototypeCarModel.gameObject.SetActive(false);
	}

	private void Start() {
		foreach (Checkpoint checkp in this.checkpoints) {
            checkp.IsVisible = false;
		}
	}

	private void Update() {
		for (int i = 0; i < cars.Count; i++) {
            if (cars[i].CarController.enabled) {
                cars[i].CarController.UpdateScore(checkpoints.GetCompletionScore, ref cars[i].CheckpointCount);

                // car crashed, change its sprite
                if (cars[i].CarController.Physics.enabled == false) {
                    cars[i].UpdateSprite(this.DeadCarSprite);
				}
                // update best car
                if (WinningCar == null || cars[i].CarController.Score > WinningCar.Score) {
                    WinningCar = cars[i].CarController;
				}
                // update second-position car
                else if (SecondPosCar == null || cars[i].CarController.Score > SecondPosCar.Score) {
                    SecondPosCar = cars[i].CarController;
				}
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
            car.UpdateSprite(this.NormalCarSprite);
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
