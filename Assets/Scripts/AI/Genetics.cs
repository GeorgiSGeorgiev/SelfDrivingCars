using System;
using System.Collections;
using System.Collections.Generic;

// Source used: Applying Evolutionary Artificial Neural Networks by ArztSamuel
// Link: https://github.com/ArztSamuel/Applying_EANNs
// I have completely rewritten the algorithm and changed a lot of things.

/// <summary>
/// The main part of the genetic algorithm.
/// </summary>
public class Genetics {
	/// <summary>
	/// The default minimum of the population values.
	/// </summary>
	public const double DefPopulationValMin = -1.0d;
	/// <summary>
	/// The default maximum of the population values.
	/// </summary>
	public const double DefPopulationValMax = 1.0d;


	/// <summary>
	/// The default probability that a population value is mutated.
	/// </summary>
	public const double DefMutationProbability = 0.4d;
	/// <summary>
	/// The default amount by which the population values are mutated. 
	/// </summary>
	public const double DefMutationAmount = 2.5d;
	/// <summary>
	/// The default percent on the new genotype that are mutated. This parameter has a big impact on the learning process.
	/// </summary>
	public const double DefMutationPercent = 5.0d;


	/// <summary>
	/// The default probability that the population values will be swapped during the crossover.
	/// </summary>
	public const double DefSwapProbability = 0.65d;
}
