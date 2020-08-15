using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Neural net + Genotype. In our case this will represent the "backend" of the car.
/// On construction sets the neural net weights to the genotype values.
/// </summary>
public class Agent: IComparable<Agent> {
	public Genotype Genotype { get; }
	public NeuralNet NeuralNet { get; }

	private bool agentLives;
	public bool AgentLives {
		get { return agentLives; }
		private set {
			if (this.agentLives == value) {
				return;
			}
			agentLives = value;
			if (AgentDiedEvent != null && !agentLives) {
				AgentDiedEvent(this);
			}
		}
	}

	/// <summary>
	/// Represents agent's death.
	/// </summary>
	public event Action<Agent> AgentDiedEvent;

	public Agent(int[] topology, Genotype genotype) {
		AgentLives = true;
		NeuralNet = new NeuralNet(topology);
		this.Genotype = genotype;

		if (NeuralNet.TotalWeightCount != this.Genotype.ValueCount) {
			throw new ArgumentException("Weight count missmatch: NeuralNet.TotalWeightCount != Genotype.ValueCount");
		}

		IEnumerator<float> genesEnum = genotype.GetEnumerator();
		foreach (NNLayer layer in NeuralNet.Layers) {
			for (int i = 0; i < layer.NodeWeights.GetLength(0); i++) {
				for (int j = 0; j < layer.NodeWeights.GetLength(1); j++) {
					genesEnum.MoveNext();
					layer.NodeWeights[i, j] = genesEnum.Current;
				}
			}
		}
	}

	public int CompareTo(Agent other) {
		return this.Genotype.CompareTo(other.Genotype);
	}

	public void ResurrectAgent() {
		this.AgentLives = true;
		//UnityEngine.Debug.Log("I LIVE!!!");
		Genotype.ResetEvalAndFitness();
	}
	
	public void KillAgent() {
		this.AgentLives = false;
	}
}
