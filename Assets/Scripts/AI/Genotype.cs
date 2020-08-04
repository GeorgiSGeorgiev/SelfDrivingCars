using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Genotype is a member of the population.
/// </summary>
public class Genotype : IComparable<Genotype>, IEquatable<Genotype>, IEnumerable<double> {
	// In evolutionary biology fitness is the quantitative representation of natural selection. (source: https://en.wikipedia.org/wiki/Fitness_(biology))
	// In our algorithm it represents the value of this genotype relative to the average value of all genotypes.
	public double Fitness { get; set; }

	private static Random rand = new Random();

	private double[] values;
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

	public Genotype(double[] newValues) {
		this.values = new double[newValues.Length];
		newValues.CopyTo(this.values, 0);
	}

	/// <summary>
	/// Generates random numbers in the interval from min to max and sets the genotype values to these numbers.
	/// </summary>
	/// <param name="min">Lower random value limit.</param>
	/// <param name="max">Upper random value limit.</param>
	public void SetRandomValues(double min, double max) {
		if (min > max) throw new ArgumentException("min value is bigger than max value!");
		double diff = max - min;
		for (int i = 0; i < values.Length; i++) {
			// NextDouble returns a double in the range from 0 to 1
			values[i] = rand.NextDouble() * diff + min; // returns a value in the range from min to max
		}
	}

	// indexer for better performance and ease of usage
	public double this[int index] {
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
	private class GenotypeEnumerator : IEnumerator<double> {
		int index;
		double[] enum_values;
		public GenotypeEnumerator(double[] values) {
			enum_values = values;
			index = -1;
		}
		public double Current { get => this.enum_values[index]; }

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

	public IEnumerator<double> GetEnumerator() {
		return new GenotypeEnumerator(this.values);
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
