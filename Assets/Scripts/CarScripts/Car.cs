using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
	public CarController CarController { get; }

	public CarPhysics CarPhysics { get => this.CarController.Physics; }

	public int CheckpointCount;

    public Car(CarController car, int checkpointCount = 0) {
		this.CarController = car;
		this.CheckpointCount = checkpointCount;
	}

	public void UpdateColor(Color color) {
		this.CarController.SpriteRenderer.color = color;
	}
}
