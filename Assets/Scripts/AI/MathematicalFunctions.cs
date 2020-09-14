using System;

/// <summary>
/// Class that contains all of the mathematical functions which are used by the neural network and the genetic algorithm.
/// </summary>
public static class MathematicalFunctions {
	/// <summary>
	/// res = <paramref name="value"/>/(1+|<paramref name="value"/>|)
	/// </summary>
	/// <param name="value"></param>
	/// <returns>The result value of the Softsign function.</returns>
	public static double Softsign(double value) {
		return value / (1 + Math.Abs(value));
	}
}
