using System;
using System.Collections;
using System.Collections.Generic;

// Source used: Applying Evolutionary Artificial Neural Networks by ArztSamuel
// Link: https://github.com/ArztSamuel/Applying_EANNs
// I have completely rewritten the algorithm and changed a lot of things.

/// <summary>
/// Delegate to a method that evaluates the current population.
/// </summary>
/// <param name="population">The population.</param>
public delegate void EvaluationMethodDel(IEnumerable<Genotype> population);

/// <summary>
/// The main part of the genetic algorithm.
/// </summary>
public class Genetics {
	/// <summary>
	/// The default minimum of the population values.
	/// </summary>
	public const float DefPopulationValMin = -1.0f;
	/// <summary>
	/// The default maximum of the population values.
	/// </summary>
	public const float DefPopulationValMax = 1.0f;


	/// <summary>
	/// The default probability that a population value is mutated.
	/// </summary>
	public const float DefMutationProbability = 0.3f;
	/// <summary>
	/// The default amount by which the population values are mutated. 
	/// </summary>
	public const float DefMutationAmount = 2f;
	/// <summary>
	/// The default percent on the new genotype that are mutated. This parameter has a big impact on the learning process.
	/// </summary>
	public const float DefMutationPercent = 1.0f;


	/// <summary>
	/// The default probability that the population values will be swapped during the crossover.
	/// </summary>
	public const float DefSwapProbability = 0.60f;

	/// <summary>
	/// Delegate to a method that evaluates the current population.
	/// </summary>
	public EvaluationMethodDel EvalMethod { get; }

	private static Random rand = new Random();

	private List<Genotype> currentPopulation;

	public int PopulationSize { get; private set; }

	public uint GenerationNumber { get; private set; }

	/// <summary>
	/// Creates a new Genetics instance.
	/// </summary>
	/// <param name="genotypeVarCount"></param>
	/// <param name="populationSize"></param>
	/// <param name="evaluationMethod"></param>
	public Genetics(int genotypeVarCount, int populationSize, EvaluationMethodDel evaluationMethod = null) {
		this.PopulationSize = populationSize;
		this.EvalMethod = evaluationMethod;

		currentPopulation = new List<Genotype>(populationSize);
		for (int i = 0; i < populationSize; i++) {
			// Add new genotypes to the current population.
			var newGenotype = new Genotype(genotypeVarCount, Genetics.DefPopulationValMin, Genetics.DefPopulationValMax, rand);
			currentPopulation.Add(newGenotype);
		}
		GenerationNumber = 1;
	}

	public static void DefaultFitnessCalculation(IEnumerable<Genotype> population) {
		int populationCount = 0;

		// Average Evaluation calculations
		float totalEval = 0;
		foreach (var genotype in population) {
			populationCount++;
			totalEval += genotype.Evaluation;
		}
		float averageEval = totalEval / populationCount;

		// Calculate Fitness
		foreach (var genotype in population) {
			genotype.CalculateFitness(averageEval);
		}
	}

	/// <summary>
	/// Gets the best four genotypes of the population.
	/// </summary>
	/// <param name="firstPopulation">The current population</param>
	/// <returns>List containing the best four genotypes.</returns>
	public static IList<Genotype> GetTheBestFourGenotypes(IList<Genotype> firstPopulation) {
		var secondPopulation = new List<Genotype> { firstPopulation[0], firstPopulation[1], firstPopulation[2], firstPopulation[3] };
		return secondPopulation;
	}

	/// <summary>
	/// Recombines the genotypes of the secondPopulation.
	/// </summary>
	/// <param name="secondPopulation">The population from which we choose the genotypes to be crossed.</param>
	/// <param name="resultPopulationSize">The result population size.</param>
	/// <returns>New population with the size of resultPopulationSize.</returns>
	public static IList<Genotype> DefCreateRandomCombination(IList<Genotype> secondPopulation, int resultPopulationSize) {
		if (secondPopulation.Count < 2) {
			throw new ArgumentException("The second population has to have at least two genotypes.");
		}

		var resultPopulation = new List<Genotype> { secondPopulation[0], secondPopulation[1] };

		while (resultPopulation.Count < resultPopulationSize) {
			// get two random indices
			int index1 = rand.Next(0, secondPopulation.Count);
			int index2 = rand.Next(0, secondPopulation.Count);
			long safetyCounter = long.MinValue;

			while (index1 == index2) {
				index2 = rand.Next(0, secondPopulation.Count);
				safetyCounter++;
				if (safetyCounter == long.MaxValue) {
					throw new Exception("Internal error, try to run the program again. Could not generate two random different indexes.");
				}
			}

			// Create random crossover
			var crossoverResult = CreateCrossover(secondPopulation[index1], secondPopulation[index2], DefSwapProbability);
			// add the items to the new population
			resultPopulation.Add(crossoverResult.Item1);
			if (resultPopulation.Count < resultPopulationSize) {
				resultPopulation.Add(crossoverResult.Item2);
			}
		}

		return resultPopulation;
	}

	public static (Genotype, Genotype) CreateCrossover(Genotype parent1, Genotype parent2, float swapProbability) {
		var newGenes1 = new float[parent1.ValueCount];
		var newGenes2 = new float[parent2.ValueCount];

		for (int i = 0; i < parent1.ValueCount; i++) {
			// swapping just some of the parameters
			if (rand.NextDouble() < swapProbability) { // the probability that rand.Next() < swapProbability is actualliy more than or equal to swapProbability
				// doing the swap
				newGenes1[i] = parent2[i];
				newGenes2[i] = parent1[i];
			}
			else {
				newGenes1[i] = parent1[i];
				newGenes2[i] = parent2[i];
			}
		}
		return (new Genotype(newGenes1), new Genotype(newGenes2));
	}

	public static void DefMutateAllExceptTheBestOne(IList<Genotype> resultpopulation) {
		for (int i = 1; i < resultpopulation.Count; i++) {
			if (rand.NextDouble() < DefMutationPercent) {
				MutateGenotype(resultpopulation[i], DefMutationProbability, DefMutationAmount);
			}
		}
	}

	/// <summary>
	/// Completely change the genotype to a random value.
	/// </summary>
	/// <param name="genotype">The genotype to be mutated.</param>
	/// <param name="mutationProbability">The probability of concrete gene of the genotype to be mutated.</param>
	/// <param name="mutationAmount">The amount by which the genotype will be mutated.</param>
	public static void MutateGenotype(Genotype genotype, double mutationProbability, double mutationAmount) {
		for (int i = 0; i < genotype.ValueCount; i++) {
			if (rand.NextDouble() < mutationAmount) {
				genotype[i] += (float)(rand.NextDouble() * 2 * mutationAmount - mutationAmount);
			}
		}
	}

	/// <summary>
	/// Start the evaluation of the population.
	/// </summary>
	public void Start() {
		EvalMethod(currentPopulation);
	}

	/// <summary>
	/// This method has to be called after the evaluation is finished to continue the process of machine learning.
	/// </summary>
	public void FinishEvolveAndStartAgain() {
		Genetics.DefaultFitnessCalculation(currentPopulation);
		currentPopulation.Sort();
		var secondPopulation = Genetics.GetTheBestFourGenotypes(currentPopulation);
		var resultPopulation = Genetics.DefCreateRandomCombination(secondPopulation, this.PopulationSize);
		Genetics.DefMutateAllExceptTheBestOne(resultPopulation);
		currentPopulation = new List<Genotype>(resultPopulation);
		GenerationNumber++;

		this.EvalMethod(currentPopulation);
	}
}