using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticsController : MonoBehaviour {
    private static uint controllersActive = 0;

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
		if (controllersActive > 0) {
			throw new Exception("More than one GenetcsController-s running at the moment.");
		}
		controllersActive++;
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

		foreach (var item in population) {
			agents.Add(new Agent(NNTopology, item));
		}

		//
		//
		//
		// I have to assign agents to cars
		// TrackManager needed
		//
		//
		//
	}

	private void OnAgentDied(Agent agent) {
		AgentsAliveCount --;

		if (AgentsAliveCount == 0) {
			NoAgentsLeft?.Invoke();
		}
	}
}
