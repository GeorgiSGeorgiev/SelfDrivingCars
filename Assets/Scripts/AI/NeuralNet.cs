using System;

public class NeuralNet {
	public NNLayer[] Layers { get; }
	public uint TotalWeightCount { get; }

	/// <summary>
	/// Creates a new fully connected feedforward NN. The netTopology represents the number of nodes in each layer.
	/// </summary>
	/// <param name="netTopology">Uint array that represents the number of nodes in each layer.</param>
	public NeuralNet(uint[] netTopology) {
		Layers = new NNLayer[netTopology.Length - 1]; // the last element of the netTopology array represents the output
		for (int i = 0; i < Layers.Length; i++) {
			Layers[i] = new NNLayer(netTopology[i], netTopology[i + 1]);
		}

		// Count the total number of weights
		for (int i = 0; i < (netTopology.Length - 1); i++) {
			TotalWeightCount += (netTopology[i] + 1) * netTopology[i + 1]; // get the number of output weights for each node
			//					+ 1 represents the bias node
		}
	}

	/// <summary>
	/// Get net inputs and calculate the outputs. This method uses the GetNewOutputs method from the NNLayer class.
	/// The default activation function is Softsign. To use different function use the next method variant.
	/// </summary>
	/// <param name="inputs">Array containing all the inputs.</param>
	/// <returns>The values after all of the calculations.</returns>
	public double[] GetTheNNOutputs(double[] inputs) {
		// inputs is passed by reference so we don't want to change it, so:
		// create the outputs array and copy inputs array to outputs array
		var outputs = new double[inputs.Length];
		inputs.CopyTo(outputs, 0);

		// get the right output values
		foreach (var item in Layers) {
			outputs = item.GetNewOutputs(outputs);
		}
		return outputs;
	}

	/// <summary>
	/// Get net inputs and calculate the outputs. This method uses the GetNewOutputs method from the NNLayer class.
	/// Allows to pick a custom activation function.
	/// </summary>
	/// <param name="inputs">Array containing all the inputs.</param>
	/// <param name="activationFunc"></param>
	/// <returns></returns>
	public double[] GetTheNNOutputs(double[] inputs, Func<double, double> activationFunc) {
		var outputs = new double[inputs.Length];
		inputs.CopyTo(outputs, 0);

		foreach (var item in Layers) {
			outputs = item.GetNewOutputs(outputs, activationFunc);
		}
		return outputs;
	}
}
