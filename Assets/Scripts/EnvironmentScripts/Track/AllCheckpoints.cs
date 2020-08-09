using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCheckpoints : IReadOnlyList<Checkpoint> {
	private Checkpoint[] checkps;

	public int Count => checkps.Length;

	public float TrackLength { get; }

	/// <summary>
	/// Create a new AllCheckpoints class and calculate the right Checkpoint values.
	/// </summary>
	/// <param name="checkpoints">Array containing all of the checkpoints.</param>
	public AllCheckpoints(Checkpoint[] checkpoints) {
		this.checkps = checkpoints;
		if (checkpoints != null && checkpoints.Length > 0) {
			checkpoints[0].DistanceFromLastOne = 0;
			checkpoints[0].DistanceFromStart = 0;
			// calculate distances
			for (int i = 1; i < this.checkps.Length; i++) {
				checkps[i].DistanceFromLastOne = Vector2.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
				checkps[i].DistanceFromStart = checkps[i].DistanceFromLastOne + checkps[i - 1].DistanceFromStart;
			}
			this.TrackLength = checkps[checkps.Length - 1].DistanceFromStart;
			// calculate score
			for (int i = 1; i < this.checkps.Length; i++) {
				checkps[i].ScoreValue = (checkps[i].DistanceFromStart / TrackLength) - checkps[i - 1].ScoreTotal;
				checkps[i].ScoreTotal = checkps[i].ScoreValue + checkps[i - 1].ScoreTotal;
			}
		}
	}

	private float CalculateScore(int index, float currentDistFromLast) {
		float completion = (checkps[index].DistanceFromLastOne - currentDistFromLast) / checkps[index].DistanceFromLastOne;
		if (completion < 0) {
			return 0;
		}
		else {
			return checkps[index].ScoreValue * completion;
		}
	}

	/// <summary>
	/// Count the checkpoints and get the complition percent for the given car.
	/// </summary>
	/// <param name="car">The car we are counting the percent on.</param>
	/// <param name="checkpInx">Index of the current checkpoint.</param>
	/// <returns></returns>
	public float GetCompletionScore(CarController car, ref int checkpInx) {
		// the finish line
		if ((checkpInx - 1) == checkps.Length) {
			return 1;
		}
		// starts new lap
		if (checkpInx == checkps.Length) {
			checkpInx = 1;
			return 1;
		}

		float toNextOne = Vector2.Distance(car.transform.position, checkps[checkpInx].transform.position);
		if (toNextOne <= checkps[checkpInx].CaptureRadius) {
			checkpInx++;
			car.CheckpointCaptured();
			return GetCompletionScore(car, ref checkpInx);
		}
		else {
			return checkps[checkpInx - 1].ScoreTotal + this.CalculateScore(checkpInx, toNextOne);
		}
	}

	public Checkpoint this[int i] {
		get => checkps[i];
	}

	public IEnumerator<Checkpoint> GetEnumerator() {
		for (int i = 0; i < checkps.Length; i++) {
			yield return checkps[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return this.GetEnumerator();
	}
}
