using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticsController : MonoBehaviour {
	public static GeneticsController Instance;

	[SerializeField]
	private int AgentCount = 40;

	[SerializeField]
	private int[] NNTopology;
	private List<Agent> agents { get; } = new List<Agent>();

	public uint AgentsAliveCount { get; private set; }

	public event Action NoAgentsLeft;

	private Genetics geneticAlg;

	public uint GenerationNumber { get => geneticAlg.GenerationNumber; }

	private void Awake() {
		if (Instance != null) {
			//throw new Exception("More than one GenetcsController-s running at the moment.");
			return;
		}
		Instance = this;
	}

	public void StartGeneticAlg() {
		var neuralNet = new NeuralNet(this.NNTopology);
		this.geneticAlg = new Genetics(neuralNet.TotalWeightCount, this.AgentCount, StartEval);
		NoAgentsLeft += geneticAlg.FinishEvolveAndStartAgain;
		geneticAlg.Start();
	}

	private void StartEval(IEnumerable<Genotype> population) {
		this.agents.Clear();
		AgentsAliveCount = 0;

		foreach (Genotype item in population) {
			agents.Add(new Agent(NNTopology, item));
		}

		TrackController.TCInstance.UpdateCarCount(this.agents.Count);
		var carsEnum = TrackController.TCInstance.GetCarEnumerator();
		// add agents to cars
		foreach (var agent in agents) {
			if (!carsEnum.MoveNext()) {
				throw new Exception("carsEnum ended before agents enum");
			}
			carsEnum.Current.Agent = agent;
			this.AgentsAliveCount++;
			agent.AgentDiedEvent += OnAgentDied;
		}

		TrackController.TCInstance.Restart();
	}

	private void OnAgentDied(Agent agent) {
		AgentsAliveCount --;

		if (AgentsAliveCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}
}
