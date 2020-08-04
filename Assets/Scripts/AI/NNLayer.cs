using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Custom-made exception
/// </summary>
public class NodeCountException: Exception {
	const string errorMessage = "Node count missmatch.";

	public NodeCountException() : base(errorMessage) {
		this.Source = "SelfDriving cars application";
	}

	public NodeCountException(string message) : base(String.Format("{0} - {1}", errorMessage, message)) {
		this.Source = "SelfDriving cars application";
	}
}

public class NNLayer {
	public uint NodeCount { get; }
	public uint OutputCount { get; }
	public double[,] NodeWeights { get; set; }
	// the first index represents the node ID, the second is the output weight index in the concrete node

	public NNLayer(uint nodeCount, uint outputCount) {
		this.NodeCount = nodeCount;
		this.OutputCount = outputCount;
		NodeWeights = new double[nodeCount + 1, outputCount]; // the 1 represents the bias node
	}


	/// <summary>
	/// Gets the new outputs of one concrete node. Firstly the method uses the weights to calculate the output values
	/// and then it aplies the Softsign function to the output which sets the values in the interval from -1 to 1.
	/// </summary>
	/// <param name="inputs">The input values that have to be processed.</param>
	/// <returns>Already calculated output values.</returns>
	public double[] GetNewOutputs(double[] inputs) {
		if (inputs.Length != NodeCount) {
			throw new NodeCountException();
		}

		double[] result = new double[OutputCount];
		uint biasedNodeCount = this.NodeCount + 1;
		var biasedInputs = new double[biasedNodeCount];
		inputs.CopyTo(biasedInputs, 0);
		biasedInputs[biasedNodeCount - 1] = 1;

		for (int i = 0; i < OutputCount; i++) {
			for (int j = 0; j < biasedNodeCount; j++) {
				result[i] += biasedInputs[j] * NodeWeights[j, i];
			}
		}

		// The result value has to be in the interval from -1 to 1. The activation function we have chosen is the Softsign.
		for (int i = 0; i < result.Length; i++) {
			result[i] = MathematicalFunctions.Softsign(result[i]); // check this out
		}

		return result;
	}


	/// <summary>
	/// Gets the new outputs of one concrete node. Firstly the method uses the weights to calculate the output values
	/// and then it aplies the activationFunc to the output which sets the values in the interval from -1 to 1.
	/// </summary>
	/// <param name="inputs">The input values that have to be processed.</param>
	/// <param name="activationFunc">The activation function to apply in the calculations of the autput.</param>
	/// <returns>Already calculated output values.</returns>
	public double[] GetNewOutputs(double[] inputs, Func<double, double> activationFunc) {
		if (inputs.Length != NodeCount) {
			throw new NodeCountException();
		}

		double[] result = new double[OutputCount];
		uint biasedNodeCount = this.NodeCount + 1;
		var biasedInputs = new double[biasedNodeCount];
		inputs.CopyTo(biasedInputs, 0);
		biasedInputs[biasedNodeCount - 1] = 1;

		for (int i = 0; i < OutputCount; i++) {
			for (int j = 0; j < biasedNodeCount; j++) {
				result[i] += biasedInputs[j] * NodeWeights[j, i];
			}
		}

		// The result value has to be in the interval from -1 to 1.
		for (int i = 0; i < result.Length; i++) {
			result[i] = activationFunc(result[i]);
		}

		return result;
	}
}
