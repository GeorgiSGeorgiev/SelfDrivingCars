using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A custom made data structure which contains all of the checkpoints of the current track.
/// <para>Unity Warning!!! Checkpoints have to be put on track in the right order. The order of the checkpoionts has to be the same as the order of their creation times. Otherwise the whole checkpoint system will not be functional.</para>
/// </summary>
public class AllCheckpoints : IReadOnlyList<Checkpoint> {
	private Checkpoint[] checkps;

	/// <summary>
	/// The number of all checkpoints.
	/// </summary>
	public int Count => checkps.Length;

	public float TrackLength { get; }

	/// <summary>
	/// Create a new <c>AllCheckpoints</c> class and calculate the right Checkpoint values.
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

	/// <summary>
	/// Calculates the current score.
	/// </summary>
	/// <param name="index">The index of the last captured checkpoint.</param>
	/// <param name="currentDistFromLast">Distance from the last captured checkpoint.</param>
	/// <returns>The result score.</returns>
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
		if (checkpInx >= checkps.Length) {
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

	/// <summary>
	/// Checkpoint indexer.
	/// </summary>
	/// <param name="i">The index of the requested checkpoint.</param>
	/// <returns></returns>
	public Checkpoint this[int i] {
		get => checkps[i];
	}

	/// <summary>
	/// Checkpoints enumerator. Enumerates a private array which contains all of the checkpoints.
	/// </summary>
	/// <returns>Current checkpoint.</returns>
	public IEnumerator<Checkpoint> GetEnumerator() {
		for (int i = 0; i < checkps.Length; i++) {
			yield return checkps[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return this.GetEnumerator();
	}
}
