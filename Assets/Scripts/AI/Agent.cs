using System;
using System.Collections.Generic;

/// <summary>
/// NeuralNet + Genotype. In our case this will represent the "backend" of the car.
/// </summary>
public class Agent: IComparable<Agent> {
	public Genotype Genotype { get; }
	public NeuralNet NeuralNet { get; }

	private bool agentLives;
	/// <summary>
	/// Determines if the current agent is still alive.
	/// </summary>
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

	/// <summary>
	/// The main Agent constructor.
	/// </summary>
	/// <param name="topology">The topology of the neural network. It is an array which contains the number of the nodes in each layer.</param>
	/// <param name="genotype">The values which will be assigned to the NN weights.</param>
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

	/// <summary>
	/// Compare this Agent's Genotype to another Agent's Genotype.
	/// </summary>
	/// <param name="other">The other Agent.</param>
	/// <returns>Positive, negative or 0 according to the comparison result.</returns>
	public int CompareTo(Agent other) {
		return this.Genotype.CompareTo(other.Genotype);
	}

	/// <summary>
	/// Resets the Agent's Genotype Evaluation and Fitness.
	/// </summary>
	public void ResurrectAgent() {
		this.AgentLives = true;
		//UnityEngine.Debug.Log("I LIVE!!!");
		Genotype.ResetEvalAndFitness();
	}
	
	/// <summary>
	/// Kills the Agent.
	/// </summary>
	public void KillAgent() {
		this.AgentLives = false;
	}
}
