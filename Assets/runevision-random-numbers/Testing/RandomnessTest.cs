/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;

public interface RandomSequence {
	uint Next ();
	void Reset ();
	string GetName ();
}

public class RandomnessTest {
	public RandomSequence sequence;
	public string name;
	public string result;
	public float[] noiseSequence = new float[256*256];
	public float[,] coordsArray = new float[256,256];
	public float[] diagonalSums = new float[256];
	public float diagonalsDeviation = 0;
	public int byteIndex = 0;
	
	public RandomnessTest (RandomSequence randomSequence) {
		sequence = randomSequence;
		name = sequence.GetName ().Replace ("#", "0\u2026n");
		Test ();
		Console.WriteLine ("\n"+name);
		Console.WriteLine (result);
	}
	
	public uint GetNext () {
		return sequence.Next ();
	}
	
	public uint GetBytePart (uint i, int byteIndex) {
		return (((i >> (8 * byteIndex)) % 256) + 256) % 256;
	}
	
	public uint GetInRange (uint i, uint range) {
		return ((i % range) + range) % range;
	}

	public void Reset () {
		sequence.Reset ();
	}

	private void Test () {
		uint[] ints = new uint[500000];
		
		// Call random function
		sequence.Reset ();
		Stopwatch stopWatch = new Stopwatch ();
		stopWatch.Start ();
		for (int i=0; i<ints.Length; i++)
			ints[i] = sequence.Next ();
		stopWatch.Stop ();
		
		// Convert to bytes
		byte[] bytes = new byte[ints.Length];
		for (int i = 0; i < bytes.Length; i++)
			bytes[i] = (byte)GetBytePart (ints[i], byteIndex);
		
		// Test randomness data
		Ent.EntCalc ent = new Ent.EntCalc (false);
		ent.AddSample (bytes, false);
		Ent.EntCalc.EntCalcResult calcResult = ent.EndCalculation ();
		
		// Create noise sequence
		for (int i=0; i<noiseSequence.Length; i++)
			noiseSequence[i] = GetBytePart (ints[i], byteIndex) / 255f;
		
		// Create coords data
		Reset ();
		float samplesPerPixelInverse = (256f * 256f) / ints.Length;
		for (int i=0; i<ints.Length; i+=6) {
			uint x = GetBytePart (GetNext (), byteIndex);
			uint y = GetBytePart (GetNext (), byteIndex);
			coordsArray[x,y] += samplesPerPixelInverse;
		}

		// Calculate coords results
		for (int j=0; j<256; j++) {
			for (int i=0; i<256; i++) {
				float value = coordsArray[i,j];
				diagonalSums[(i + j) % 256] += value * 0.5f;
				diagonalSums[((i - j) + 256) % 256] += value * 0.5f;
			}
		}
		diagonalsDeviation = StandardDeviation (new List<float> (diagonalSums));

		// Get string with result
		result = GetResult (calcResult, stopWatch.ElapsedMilliseconds);
	}

	public static float StandardDeviation (List<float> valueList) {
		float m = 0.0f;
		float s = 0.0f;
		int k = 1;
		foreach (float value in valueList) 
		{
			float tmpM = m;
			m += (value - tmpM) / k;
			s += (value - tmpM) * (value - m);
			k++;
		}
		return (float)System.Math.Sqrt (s / (k-1));
	}
	
	string GetResult (Ent.EntCalc.EntCalcResult result, float duration) {
		double meanValueQuality = Clamp01 (1 - Math.Abs (127.5 - result.Mean) / 128);
		double serialCorrelationQuality = Clamp01 (1 - 2 * Math.Abs (result.SerialCorrelation));
		double piQuality = Clamp01 (1 - 10 * result.MonteCarloErrorPct);
		double diagonalsDeviationQuality = Clamp01 (1 - diagonalsDeviation / 256);
		double combined = Math.Min (Math.Min (Math.Min (meanValueQuality, serialCorrelationQuality), piQuality), diagonalsDeviationQuality);

		return string.Format (
			  "                             value quality\n"
			+ "Mean Value:               {0,8:F4} {1,7:P0}\n"
			+ "Serial Correlation:       {2,8:F4} {3,7:P0}\n"
			+ "Monte Carlo Pi Value:     {4,8:F4} {5,7:P0}\n"
			+ "Diagonals Deviation:      {6,8:F4} {7,7:P0}\n"
			+ "<b>Overall Quality:                   {8,7:P0}</b>\n"
			+ "\n"
			+ "Execution Time:                  {9,6} ms",

			result.Mean, meanValueQuality,
			Math.Max (0, result.SerialCorrelation), serialCorrelationQuality,
			result.MonteCarloPiCalc, piQuality,
			diagonalsDeviation, diagonalsDeviationQuality,
			combined,
			duration
		);
	}
	
	private static double Clamp01 (double val) {
		return Clamp (val, 0, 1);
	}
	
	private static double Clamp (double val, double min, double max) {
		return Math.Min (max, Math.Max (val, min));
	}
}
