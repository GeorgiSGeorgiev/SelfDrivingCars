using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One of the main simulation controllers. Controlls all of the cars and updates their stats.
/// <c>TrackController</c> gets directly the track's checkpoints and counts the track's length. It tracks the best two cars too.
/// <para><c>TrackController</c> is singleton so it has a static instance of itself inside. Check the static <paramref name ="Instance"/> property.</para>
/// </summary>
public class TrackController : MonoBehaviour {
    private static TrackController instance;
    /// <summary>
    /// Singleton instance of the TrackController. Can be accessed from everywhere if a <c>TrackController</c> was assigned to the current scene.
    /// </summary>
    public static TrackController Instance {
        get => instance;
        private set {
            if (instance != null) {
                throw new Exception("The TrackController.Instance was already created.");
            }
            instance = value;
        }
    }

    /// <summary>
    /// The velocity and steering text boxes. To be assigned from the Unity editor.
    /// </summary>
    public VelocityAndSteering stats;

    private int generationCounter = 0;
    /// <summary>
    /// Current generation/run.
    /// </summary>
    public int GenerationCounter { 
        get => generationCounter;
        private set => generationCounter = value;
    }

    /// <summary>
    /// The text box which shows the number of the current generation/run.
    /// </summary>
    public Text GenerationTextBox;

    // all checkpoints which are get directly from the track
    private AllCheckpoints checkpoints;

    /// <summary>
    /// Contains the length of the track if any checkpoints are present.
    /// </summary>
    public float TrackLength {
        get {
            if (this.checkpoints != null) {
                return checkpoints.TrackLength;
            }
            Debug.LogError("Couldn't measure the track length because no checkpoint were assigned to the track. To fix the error add checkpoints all along the track.");
            return 0;
        }
    }

    /// <summary>
    /// Car that we clone. It is used as a main agent car model.
    /// </summary>
    public CarController PrototypeCarModel;
    /// <summary>
    /// The car that player drives.
    /// </summary>
    public CarController PlayerCarModel;
    private int PlayerCheckpointCount = 1;

    private Vector3 startingPos;
    private Quaternion startingRot;

    /// <summary>
    /// List of all cars on the track. Can not be set from outside the <paramref name = "TrackController"</paramref> class.
    /// </summary>
    public List<Car> Cars { get; private set; } = new List<Car>();

    private CarController exportCar;
    /// <summary>
    /// Contains the best agent car which can be exported via serialization.
    /// </summary>
    public CarController ExportCar {
        get {
            if (this.GenerationCounter <= 1) {
                return this.WinningCar;
			}
            return this.exportCar;
        }
        private set {
            this.exportCar = value;
        }
    }

    private CarController winningCar;
    /// <summary>
    /// The best car in the race.
    /// </summary>
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
    /// <summary>
    /// The second best car.
    /// </summary>
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
			}
		}
	}
    
    /// <summary>
    /// Gets the TrackController's singleton instance, all of the checkpoints from the track, sets the player input setting of the user's car and disables the prototype car model.
    /// </summary>
	public void Awake() {
        TrackController.Instance = this;

        this.GenerationCounter = 0;
        this.GenerationTextBox.text = this.generationCounter.ToString();

        this.checkpoints = new AllCheckpoints(GetComponentsInChildren<Checkpoint>());

        if (SettingsMenu.PlayerInput) {
            this.PlayerCarModel.KeyboardInput = SettingsMenu.PlayerInput;
        }
        else {
            this.PlayerCarModel.gameObject.SetActive(false);
		}

        this.startingPos = PrototypeCarModel.transform.position;
        this.startingRot = PrototypeCarModel.transform.rotation;
        PrototypeCarModel.gameObject.SetActive(false);
	}

    /// <summary>
    /// Before the first Update all of the checkpoints are made invisible.
    /// </summary>
	public void Start() {
		foreach (Checkpoint checkp in this.checkpoints) {
            checkp.IsVisible = false;
		}
    }

    /// <summary>
    /// Unity method that updates car scores every frame.
    /// </summary>
    public void FixedUpdate() {
        if (!SettingsMenu.PlayerInput) {
            UpdateAgentCarsScore();
		}
        else {
            UpdateAgentAndPlayerCarsScore();
        }
    }

    /// <summary>
    /// Updates car scores, the best car and the second best car.
    /// </summary>
    public void UpdateAgentCarsScore() {
        for (int i = 0; i < Cars.Count; i++) {
            if (Cars[i].CarController.enabled) {
                Cars[i].CarController.UpdateScore(checkpoints.GetCompletionScore, ref Cars[i].CheckpointCount);
                // update best car
                if (WinningCar == null || Cars[i].Score > WinningCar.Score) {
                    WinningCar = Cars[i].CarController;
                }
                // update second-position car
                if ((SecondPosCar == null || Cars[i].Score > SecondPosCar.Score) && Cars[i].CarController != WinningCar) {
                    SecondPosCar = Cars[i].CarController;
                }
            }
            // car crashed, change its sprite
            else if (!(Cars[i].CarController == WinningCar || Cars[i].CarController == SecondPosCar)) {
                Cars[i].UpdateColor(Color.red);
            }
        }
    }

    /// <summary>
    /// Updates car scores, the best car and the second best car. Counts with player input.
    /// </summary>
    public void UpdateAgentAndPlayerCarsScore() {
        this.PlayerCarModel.UpdateScore(checkpoints.GetCompletionScore, ref PlayerCheckpointCount);
        for (int i = 0; i < Cars.Count; i++) {
            if (Cars[i].CarController.enabled) {
                Cars[i].CarController.UpdateScore(checkpoints.GetCompletionScore, ref Cars[i].CheckpointCount);
                // update best car
                if (WinningCar == null || Cars[i].Score > WinningCar.Score) {
                    WinningCar = Cars[i].CarController;
                }
                if (PlayerCarModel.Score > WinningCar.Score) {
                    WinningCar = PlayerCarModel;
				}
                // update second-position car
                if ((SecondPosCar == null || Cars[i].Score > SecondPosCar.Score) && Cars[i].CarController != WinningCar) {
                    SecondPosCar = Cars[i].CarController;
                }
                if ((SecondPosCar == null || PlayerCarModel.Score > SecondPosCar.Score) && PlayerCarModel != WinningCar) {
                    SecondPosCar = PlayerCarModel;
				}
            }
            // car crashed, change its sprite
            else if (!(Cars[i].CarController == WinningCar || Cars[i].CarController == SecondPosCar)) {
                Cars[i].UpdateColor(Color.red);
            }
        }
    }

    /// <summary>
    /// Sets the right car count on the track according to the method's input parameter.
    /// </summary>
    /// <param name="carCount">The new car count.</param>
    public void UpdateCarCount(int carCount) {
        if (carCount <= 0) {
            throw new ArgumentException("carCount must not be less that or equal to zero");
        }
        if (carCount == Cars.Count) { return; }
        // remove cars
        if (carCount < Cars.Count) {
            int removeCount = Cars.Count - carCount;
            for (; removeCount > 0; removeCount--) {
                Car car = Cars[Cars.Count - 1];
                Cars.RemoveAt(Cars.Count - 1);
                Destroy(car.CarController.gameObject);
			}
            return;
		}
        // add cars
        if (carCount > Cars.Count) {
            int addCount = carCount - Cars.Count;
            for (; addCount > 0; addCount--) {
                GameObject newCar = Instantiate(PrototypeCarModel.gameObject);
                newCar.transform.position = startingPos;
                newCar.transform.rotation = startingRot;
                CarController carController = newCar.GetComponent<CarController>();
                // add the new car controller to the game logic
                Cars.Add(new Car(carController));
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
        this.exportCar = this.winningCar;
        InstantRestart();
	}

    // resets all of the cars
    private void InstantRestart() {
        foreach (Car car in Cars) {
            this.CarRestart(car.CarController);
            car.CheckpointCount = 1;
        }
        if (SettingsMenu.PlayerInput) {
            this.CarRestart(this.PlayerCarModel);
            this.PlayerCheckpointCount = 1;
		}

        WinningCar = null;
        SecondPosCar = null;
    }

    // resets a single car
    private void CarRestart(CarController car) {
        car.transform.position = startingPos;
        car.transform.rotation = startingRot;
        car.Restart();
        car.SpriteRenderer.color = Color.white;
    }

    /// <summary>
    /// Manages genotype export. Serializes and saves the best genotype to a file.
    /// <para>As a genotype name the default one is used.</para>
    /// </summary>
    public void ExportTheBestGenotype() {
        if (SettingsMenu.PlayerInput) {
            return;
        }
        if (this.WinningCar == null) {
            throw new Exception("The winning car was not still set. Wait is needed. Skip the first 3-4 (maybe more) frames.");
		}
        this.WinningCar.Agent.Genotype.SaveToFile();
	}

    /// <summary>
    /// Manages genotype export. Serializes and saves the best genotype to a file.
    /// <para>As a genotype name is used the <paramref name="agentName"></paramref> parameter.</para>
    /// </summary>
    /// <param name="agentName">The genotype name.</param>
    public void ExportTheBestGenotype(string agentName) {
        if (SettingsMenu.PlayerInput) {
            return;
		}
        if (this.WinningCar == null) {
            throw new Exception("The winning car was not still set. Wait is needed. Skip the first 3-4 frames (maybe more).");
        }
        this.WinningCar.Agent.Genotype.SaveToFile(agentName);
    }

    /// <summary>
    /// Enumerator that returns each <c>Car</c> from the game.
    /// </summary>
    /// <returns>The current <c>Car</c>.</returns>
    public IEnumerator<CarController> GetCarEnumerator() {
        for (int i = 0; i < Cars.Count; i++)
            yield return Cars[i].CarController;
    }
}
