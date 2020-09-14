using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class managing the overall simulation. Manages the start of the genetic algorithm, gets the preloaded genotypes and manages the camera updates.
/// <para>Updates the in-game UI too.</para>
/// </summary>
public class GameController : MonoBehaviour {
    // The main camera object, to be referenced in Unity Editor.
    public CameraSettings MainCamera;
    public MinimapCameraSettings MinimapCamera;
    private CarController targetCar;
    /// <summary>
    /// Velocity and steering textboxes. They have to be referenced from the Unity editor as a part of the VelocityAndSteering class.
    /// </summary>
    public VelocityAndSteering stats;

    /// <summary>
    /// Static Queue containing all of the preloaded genotypes.
    /// </summary>
    public static Queue<Genotype> PreloadedGenotypes;

    public Text CarIDTextBox;
    public Text CarScoreTextBox;

    /// <summary>
    /// The speedometer sliders.
    /// </summary>
    public SlidersController SlidersController;

    /// <summary>
    /// Sets the camera mode, the speedometer, the preloaded genotypes and starts the genetic algorithm.
    /// </summary>
    public void Start() {
        this.MainCamera.cameraMode = SettingsMenu.CurrentCameraMode; 
        if (!SettingsMenu.PlayerInput) {
            TrackController.Instance.WinningCarHasChanged += OnBestCarChanged;
        }
        this.SlidersController.SetParameters(CarPhysics.MaximalForwardsVelocity);
        if (!SettingsMenu.PlayerInput) {
            PreloadedGenotypes = SettingsMenu.ImportedGenotypes;
        }

        if (PreloadedGenotypes != null) {
            GeneticsController.Instance.StartGeneticAlg(PreloadedGenotypes);
        }
        else {
            GeneticsController.Instance.StartGeneticAlg();
        }
    }

    // Callback method for when the best car has changed.
    private void OnBestCarChanged(CarController bestCar) {
        if (bestCar != null) {
            this.ChangeFocus(bestCar);
        }
    }

    // Change the camera focus to the newCarTarget.
    private void ChangeFocus(CarController newCarTarget) {
        this.MainCamera.Target = newCarTarget.gameObject.transform;
        this.MinimapCamera.Target = newCarTarget.gameObject.transform;
        this.CarIDTextBox.text = newCarTarget.ID.ToString();
        this.CarScoreTextBox.text = newCarTarget.Score.ToString();
        this.stats.CarPhysics = newCarTarget.Physics;
        this.SlidersController.SetValue(this.stats.CarPhysics.Velocity);
        this.targetCar = newCarTarget;
    }

    /// <summary>
    /// Changes the camera focus and updates the car score text on the UI if neccesary.
    /// </summary>
    private void Update() {
        ChangeCameraToBestFunctionalCar();
        if (this.targetCar != null) {
            this.CarScoreTextBox.text = this.targetCar.Score.ToString();
        } 
        this.SlidersController.SetValue(this.stats.CarPhysics.Velocity);
    }

    /// <summary>
    /// Changes the camera focus to the best functional car.
    /// </summary>
    public void ChangeCameraToBestFunctionalCar() {
        if (!SettingsMenu.PlayerInput) {
            if (TrackController.Instance.WinningCar != null && TrackController.Instance.WinningCar.Physics.enabled == false) {
                CarController bestMovingCar = this.GetBestFunctionalCar();
                //Debug.Log(bestMovingCar.ID);
                if (bestMovingCar != null && bestMovingCar != this.targetCar) {
                    this.ChangeFocus(bestMovingCar);
                }
            }
        }
        else { // player online... set the camera to the player
            if (TrackController.Instance.PlayerCarModel.enabled == true) {
                this.ChangeFocus(TrackController.Instance.PlayerCarModel);
            }
            else {
                CarController bestMovingCar = this.GetBestFunctionalCar();
                if (bestMovingCar != null) {
                    this.ChangeFocus(bestMovingCar);
                }
            }
        }
    }

    private CarController GetBestFunctionalCar() {
        CarController BestMovingCar = TrackController.Instance.Cars[0].CarController;

        float bestScore = 0;
        foreach (var car in TrackController.Instance.Cars) {
            if (car.CarPhysics.enabled && car.Score > bestScore) {
                BestMovingCar = car.CarController;
                bestScore = BestMovingCar.Score;
            }

        }
        return BestMovingCar;
    }
}