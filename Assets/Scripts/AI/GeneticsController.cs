using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class that controlls the genetic algorithm. It tracks the agent count and starts the Genetics.
/// <para>GeneticsController works directly with the <c>TrackController</c> from which it gets all of the agents and updates them between the different runs.</para>
/// </summary>
public class GeneticsController : MonoBehaviour {
	private static GeneticsController instance;
	/// <summary>
	/// The singleton property.
	/// </summary>
	public static GeneticsController Instance {
		get => instance;
		private set {
			if (instance != null) {
				throw new Exception("More than one GenetcsController-s running at the moment.");
			}
			instance = value;
		}
	}

	public int AgentCount { get; private set; } = 42;
	public int PlayersCount { get; private set; } = 0;

	/// <summary>
	/// Number of nodes in each layer of the Agent's NeuralNet.
	/// </summary>
	public int[] NNTopology;
	private List<Agent> agents { get; } = new List<Agent>();

	public uint AgentsAliveCount { get; private set; }

	private event Action NoAgentsLeft;

	private Genetics geneticAlg;

	/// <summary>
	/// Creates a new Instance and sets the AgentCount.
	/// </summary>
	public void Awake() {
		GeneticsController.Instance = this;
		if (!SettingsMenu.PlayerInput) {
			this.AgentCount = SettingsMenu.AgentCount;
		}
		else {
			this.AgentCount = 1;
		}
	}

	private void Start() {
		// cars with agents don't need to have asigned the TimedOut event because they have the agent died event
		// the player car has no agent so it needs somehow to detect when the car time runs out
		TrackController.Instance.PlayerCarModel.CarExploded += OnPlayerDied;
	}


	// Main algorithm entry points

	/// <summary>
	/// Creates a new <c>NeuralNet</c>, a new <c>Genetics</c> and starts the Genetic algorithm.
	/// </summary>
	public void StartGeneticAlg() {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain; // SettingsMenu.PlayerInput == false here
		geneticAlg.Start();
	}

	/// <summary>
	/// Creates a new <c>NeuralNet</c>, a new <c>Genetics</c> and starts the Genetic algorithm. Also gets preloaded Genotypes.
	/// </summary>
	/// <param name="preloadedGenotypes">Preloaded genotypes.</param>
	public void StartGeneticAlg(Queue<Genotype> preloadedGenotypes) {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, preloadedGenotypes, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain;
		geneticAlg.Start();
	}

	// Important method that controlls the start of the evaluation of all
	// Warning! This method operates with the TrackController.
	private void StartEval(IEnumerable<Genotype> population) {
		this.agents.Clear();
		AgentsAliveCount = 0;
		PlayersCount = 0;

		// creates new agents from the NNs and the genotypes and adds them to the agents List
		foreach (Genotype genotype in population) {
			agents.Add(new Agent(NNTopology, genotype));
		}

		TrackController.Instance.UpdateCarCount(this.agents.Count);
		var carsEnum = TrackController.Instance.GetCarEnumerator();
		// add agents to cars
		foreach (var agent in agents) {
			if (!carsEnum.MoveNext()) {
				throw new Exception("carsEnum ended before agents enum");
			}
			carsEnum.Current.Agent = agent;
			this.AgentsAliveCount++;
			agent.AgentDiedEvent += OnAgentDied;
		}
		if (SettingsMenu.PlayerInput) {
			PlayersCount++;
		}

		TrackController.Instance.Restart();
	}

	private void OnAgentDied(Agent agent) {
		AgentsAliveCount--;
		if (AgentsAliveCount == 0 && PlayersCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}

	private void OnPlayerDied() {
		PlayersCount--;
		if (AgentsAliveCount == 0 && PlayersCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}
}
