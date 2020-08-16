using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// Genotype is a member of the population.
/// </summary>
[Serializable]
public class Genotype : IComparable<Genotype>, IEquatable<Genotype>, IEnumerable<float> {
	public float Evaluation { get; set; }
	// In evolutionary biology fitness is the quantitative representation of natural selection. (source: https://en.wikipedia.org/wiki/Fitness_(biology))
	// In our algorithm it represents the value of this genotype relative to the average value of all genotypes.
	// Fitness = Evaluation / averageEval 
	public float Fitness { get; private set; }

	private static System.Random rand = new System.Random();

	private float[] values;
	public int ValueCount {
		get {
			if (this.values == null) {
				return 0;
			}
			else {
				return this.values.Length;
			}
		}
	}

	public static readonly string DefaultPathAndName = Application.persistentDataPath + "/save.dat";
	public static string LastSavedTo { get; private set; }

	public Genotype(float[] newValues) {
		this.values = new float[newValues.Length];
		newValues.CopyTo(this.values, 0);
	}

	/// <summary>
	/// The new genotype values are random numbers from the given interval.
	/// </summary>
	public Genotype(int genotypeVarCount, float populationValMin, float populationValMax, System.Random random) {
		rand = random;
		this.values = new float[genotypeVarCount];
		this.SetRandomValues(populationValMin, populationValMax);
	}

	/// <summary>
	/// Generates random numbers in the interval from min to max and sets the genotype values to these numbers.
	/// </summary>
	/// <param name="min">Lower random value limit.</param>
	/// <param name="max">Upper random value limit.</param>
	public void SetRandomValues(float min, float max) {
		if (min > max) throw new ArgumentException("min value is bigger than max value!");
		float diff = max - min;
		//debug
		string debugString = "";
		for (int i = 0; i < values.Length; i++) {
			// NextDouble returns a double in the range from 0 to 1
			values[i] = (float) rand.NextDouble() * diff + min; // returns a value in the range from min to max
			debugString += values[i].ToString() + "  ";
		}
		//Debug.Log("Weights:" + debugString);
	}

	/// <summary>
	/// Calculates the Genotype Fitness using the formula Fitness = Evaluation / averageEval
	/// </summary>
	/// <param name="averagePopulationEvaluation">The average population evaluation where averageEval = totalEval / populationCount.</param>
	public void CalculateFitness(float averagePopulationEvaluation) {
		this.Fitness = this.Evaluation / averagePopulationEvaluation;
	}

	public void ResetEvalAndFitness() {
		this.Evaluation = 0;
		this.Fitness = 0;
	}

	public string GenotypeValuesToString() {
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.values.Length; i++) {
			stringBuilder.Append(this.values[i]);
			if (i != this.values.Length - 1) {
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}
	
	public void SaveToFile() {
		string targetPath = Genotype.DefaultPathAndName;
		FileStream file;

		if (File.Exists(targetPath)) {
			file = File.OpenWrite(targetPath);
		}
		else {
			file = File.Create(targetPath);
		}
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, this);
		file.Flush();
		file.Close();
	}

	public void SaveToFile(string agentName) {
		string targetPath = Application.persistentDataPath + $"/save_{ agentName }.dat";
		Genotype.LastSavedTo = targetPath;
		FileStream file;

		if (File.Exists(targetPath)) {
			file = File.OpenWrite(targetPath);
		}
		else {
			file = File.Create(targetPath);
		}
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, this);
		file.Flush();
		file.Close();
	}

	public static Genotype LoadFromFile() {
		string targetPath = Genotype.DefaultPathAndName;
		FileStream file;

		if (File.Exists(targetPath)) {
			file = File.OpenRead(targetPath);
		}
		else {
			throw new FileNotFoundException();
		}

		BinaryFormatter bf = new BinaryFormatter();
		Genotype resultGenotype;
		resultGenotype = (Genotype)bf.Deserialize(file);
		file.Close();
		return resultGenotype;
	}

	public static Genotype LoadFromFile(string agentName) {
		string targetPath = Application.persistentDataPath + $"/save_{ agentName }.dat";
		FileStream file;

		if (File.Exists(targetPath)) {
			file = File.OpenRead(targetPath);
		}
		else {
			throw new FileNotFoundException();
		}

		BinaryFormatter bf = new BinaryFormatter();
		Genotype resultGenotype;
		resultGenotype = (Genotype)bf.Deserialize(file);
		file.Close();
		return resultGenotype;
	}

	/// <summary>
	/// Loades a seriallized agent from concrete path.
	/// </summary>
	/// <param name="path">Full path to the agent.</param>
	/// <param name="fullName">Full file name without the '/' symbol at the beginning and without the file extension.</param>
	/// <returns></returns>
	public static Genotype LoadFromFile(string path, string fullName) {
		string targetPath = path + $"/{ fullName }.dat";
		FileStream file;

		if (File.Exists(targetPath)) {
			file = File.OpenRead(targetPath);
		}
		else {
			throw new FileNotFoundException();
		}

		BinaryFormatter bf = new BinaryFormatter();
		Genotype resultGenotype;
		resultGenotype = (Genotype)bf.Deserialize(file);
		file.Close();
		return resultGenotype;
	}



	// indexer for better performance and ease of usage
	public float this[int index] {
		get => this.values[index];
		set => this.values[index] = value;
	}

	/// <summary>
	/// Compares the fitness values of this genotype and of another genotype.
	/// </summary>
	/// <param name="other">The genotype to compare with.</param>
	/// <returns>Negative, positive value or 0 according to the result of the comparison.</returns>
	public int CompareTo(Genotype other) {
		return other.Fitness.CompareTo(this.Fitness);
	}

	public bool Equals(Genotype other) {
		return this.Fitness == other.Fitness;
	}

	public override bool Equals(object obj) {
		if ((obj == null) || !this.GetType().Equals(obj.GetType())) {
			return false;
		}
		else {
			Genotype gen1 = (Genotype)obj;
			return this.Fitness == gen1.Fitness;
		}
	}

	public override int GetHashCode() {
		return this.Fitness.GetHashCode();
	}

	#region Comparison operators
	public static bool operator <(Genotype gen1, Genotype gen2) {
		return gen1.Fitness < gen2.Fitness;
	}
	public static bool operator >(Genotype gen1, Genotype gen2) {
		return gen1.Fitness > gen2.Fitness;
	}
	public static bool operator !=(Genotype gen1, Genotype gen2) {
		return gen1.Fitness != gen2.Fitness;
	}
	public static bool operator ==(Genotype gen1, Genotype gen2) {
		return gen1.Fitness == gen2.Fitness;
	}
	public static bool operator <=(Genotype gen1, Genotype gen2) {
		return gen1.Fitness <= gen2.Fitness;
	}
	public static bool operator >=(Genotype gen1, Genotype gen2) {
		return gen1.Fitness >= gen2.Fitness;
	}
	#endregion

	// Custom-made enumerator
	private class GenotypeEnumerator : IEnumerator<float> {
		int index;
		float[] enum_values;
		public GenotypeEnumerator(float[] values) {
			enum_values = values;
			index = -1;
		}
		public float Current { get => this.enum_values[index]; }

		object IEnumerator.Current => this.Current;

		public void Dispose() { }

		public bool MoveNext() {
			if (index == -1) {
				index = 0;
			}
			else {
				index++;
			}
			return index < enum_values.Length;
		}

		public void Reset() {
			throw new InvalidOperationException("Reset is not a valid method.");
		}
	}

	public IEnumerator<float> GetEnumerator() {
		return new GenotypeEnumerator(this.values);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
