using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticsController : MonoBehaviour {
	public static GeneticsController Instance;

	public int AgentCount = 42;
	public int PlayersCount = 0;

	public int[] NNTopology;
	private List<Agent> agents { get; } = new List<Agent>();

	public uint AgentsAliveCount { get; private set; }

	public event Action NoAgentsLeft;

	private Genetics geneticAlg;

	public void Awake() {
		if (Instance != null) {
			throw new Exception("More than one GenetcsController-s running at the moment.");
		}
		Instance = this;
		if (!SettingsMenu.PlayerInput) {
			this.AgentCount = SettingsMenu.AgentCount;
		}
		else {
			this.AgentCount = 1;
		}
	}

	private void Start() {
		// cars with agents don't need to have asigned the TimedOut event because they have agent died event
		// the player car has no agent so it needs somehow to detect when the car time runs out
		TrackController.TC.PlayerCarModel.CarExploded += OnPlayerDied;
	}

	// Main algorithm entry points
	public void StartGeneticAlg() {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain; // SettingsMenu.PlayerInput == false here
		geneticAlg.Start();
	}

	public void StartGeneticAlg(Queue<Genotype> preloadedGenotypes) {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, preloadedGenotypes, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain;
		geneticAlg.Start();
	}

	// Important method that controlls the start of the evaluation of all
	private void StartEval(IEnumerable<Genotype> population) {
		this.agents.Clear();
		AgentsAliveCount = 0;
		PlayersCount = 0;
		//Debug.Log("Error, the algorithm is on");
		foreach (Genotype genotype in population) {
			agents.Add(new Agent(NNTopology, genotype));
		}
		//Debug.Log("Car init");
		TrackController.TC.UpdateCarCount(this.agents.Count);
		var carsEnum = TrackController.TC.GetCarEnumerator();
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
		//Debug.Log(AgentsAliveCount);

		TrackController.TC.Restart();
	}

	private void OnAgentDied(Agent agent) {
		AgentsAliveCount --;
		//Debug.Log("AgentDied");
		//Debug.Log(AgentsAliveCount);
		if (AgentsAliveCount == 0 && PlayersCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}

	private void OnPlayerDied() {
		PlayersCount--;
		//Debug.Log("PlayerDied");
		//Debug.Log(PlayersCount);
		if (AgentsAliveCount == 0 && PlayersCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}
}
