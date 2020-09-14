using UnityEngine;

/// <summary>
/// Represents the whole car. Contains <c>CarController</c>,<c>CarPhysics</c> instances and the number of the checkpoints which the car has captured.
/// </summary>
public class Car {
	public CarController CarController { get; }

	public CarPhysics CarPhysics { get => this.CarController.Physics; }

	public float Score { get => this.CarController.Score; }

	public int CheckpointCount;

	/// <summary>
	/// Creates a new car.
	/// </summary>
	/// <param name="car">The controller of the car.</param>
	/// <param name="checkpointCount">The number of the captured checkpoints by this cars. The default value of this parameter is 0.</param>
    public Car(CarController car, int checkpointCount = 0) {
		this.CarController = car;
		this.CheckpointCount = checkpointCount;
	}

	/// <summary>
	/// Updates the color of the car sprite (of the car visual model).
	/// </summary>
	/// <param name="color">The new color.</param>
	public void UpdateColor(Color color) {
		this.CarController.SpriteRenderer.color = color;
	}
}
