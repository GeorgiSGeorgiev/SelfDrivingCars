using UnityEngine;

public class Checkpoint : MonoBehaviour {
    public double CaptureRadius = 4;
	public float ScoreValue;
    private SpriteRenderer spriteRenderer;
    public float DistanceFromLastOne { get; set; }
	private void Awake() {
		this.spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public double CalculateScore(double currentDistFromLast) {
		double completion = (this.DistanceFromLastOne - currentDistFromLast) / this.DistanceFromLastOne;
		if (completion < 0) {
			return 0;
		}
		else {
			return ScoreValue * completion;
		}
	}
}
