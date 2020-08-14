﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton class managing the overall simulation.
/// </summary>
public class GameController : MonoBehaviour {
    // The main camera object, to be referenced in Unity Editor.
    public CameraSettings MainCamera;
    public MinimapCameraSettings MinimapCamera;
    private CarController targetCar; 
    public VelocityAndSteering stats;

    public Queue<Genotype> PreloadedGenotypes;

    public Text CarIDTextBox;
    public Text CarScoreTextBox;

    public SlidersController SlidersController;

    public void Start() {
        TrackController.TC.WinningCarHasChanged += OnBestCarChanged;

        this.PreloadedGenotypes = SettingsMenu.ImportedGenotypes;
        if (PreloadedGenotypes != null) {
            GeneticsController.Instance.StartGeneticAlg(this.PreloadedGenotypes);
		}
        else {
            GeneticsController.Instance.StartGeneticAlg();
        }
        this.SlidersController.SetParameters(CarPhysics.MaximalForwardsVelocity);
    }

    // Callback method for when the best car has changed.
    private void OnBestCarChanged(CarController bestCar) {
        if (bestCar != null) {
            this.ChangeFocus(bestCar);
        }
    }

    private void ChangeFocus(CarController newCarTarget) {
        this.MainCamera.Target = newCarTarget.gameObject.transform;
        this.MinimapCamera.Target = newCarTarget.gameObject.transform;
        this.CarIDTextBox.text = newCarTarget.ID.ToString();
        this.CarScoreTextBox.text = newCarTarget.Score.ToString();
        this.stats.CarPhysics = newCarTarget.Physics;
        this.SlidersController.SetValue(this.stats.CarPhysics.Velocity);
        this.targetCar = newCarTarget;
    }

    private void Update() {
        ChangeCameraToBestFunctionalCar();
        if (this.targetCar != null) {
            this.CarScoreTextBox.text = this.targetCar.Score.ToString();
        } 
        this.SlidersController.SetValue(this.stats.CarPhysics.Velocity);
    }

    public void ChangeCameraToBestFunctionalCar() {
        if (TrackController.TC.WinningCar != null && TrackController.TC.WinningCar.Physics.enabled == false) {
            CarController bestMovingCar = this.GetBestFunctionalCar();
            //Debug.Log(bestMovingCar.ID);
            if (bestMovingCar != null && bestMovingCar != this.targetCar) {
                this.ChangeFocus(bestMovingCar);
            }
        }
    }

    private CarController GetBestFunctionalCar() {
        CarController BestMovingCar = TrackController.TC.cars[0].CarController;

        float bestScore = 0;
        foreach (var car in TrackController.TC.cars) {
            if (car.CarController.Physics.enabled && car.CarController.Score > bestScore) {
                BestMovingCar = car.CarController;
                bestScore = BestMovingCar.Score;
            }

        }
        return BestMovingCar;
    }
}