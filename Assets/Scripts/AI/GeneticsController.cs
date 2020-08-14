using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticsController : MonoBehaviour {
	public static GeneticsController Instance;

	public int AgentCount = 42;

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
		this.AgentCount = SettingsMenu.AgentCount;
	}

	public void StartGeneticAlg() {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain;
		geneticAlg.Start();
	}

	public void StartGeneticAlg(Queue<Genotype> preloadedGenotypes) {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, preloadedGenotypes, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain;
		geneticAlg.Start();
	}

	private void StartEval(IEnumerable<Genotype> population) {
		this.agents.Clear();
		AgentsAliveCount = 0;

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

		TrackController.TC.Restart();
	}

	private void OnAgentDied(Agent agent) {
		AgentsAliveCount --;

		if (AgentsAliveCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}
}
