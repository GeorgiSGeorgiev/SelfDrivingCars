using System;
using System.Collections;
using System.Collections.Generic;

public static class MathematicalFunctions {
	public static double Softsign(double value) {
		return value / (1 + Math.Abs(value));
	}
}
