using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
	public CarController CarController { get; set; }
	public int CheckpointCount;
	//public int DistanceFromStart { get; set; } = 0;

    public Car(CarController car = null, int checkpointCount = 0) {
		this.CarController = car;
		this.CheckpointCount = checkpointCount;
	}

	public void UpdateColor(Color color) {
		this.CarController.SpriteRenderer.color = color;
	}
}
