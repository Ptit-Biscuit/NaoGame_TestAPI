/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class RandomnessTester {
	public List<RandomnessTest> tests = new List<RandomnessTest> ();

	public RandomnessTester () {
		HashWrapper linear = new HashWrapper (
			"Linear function i * 19969 / 207",
			false,
			seed => null,
			(gen, index) => (uint)index * 19969 / 207);
		
		RNGWrapper systemRandom = new RNGWrapper (
			"System.Random",
			true,
			seed => new Random (seed),
			gen => (uint)((Random)gen).Next ());
		
		RNGWrapper systemRandomScrambled = new RNGWrapper (
			"System.Random (scrambled)",
			true,
			seed => new Random ((int)((seed ^ 0x5DEECE66DL) & ((1L << 48) - 1))),
			gen => (uint)((Random)gen).Next ());
		
		RNGWrapper xorShift = new RNGWrapper (
			"XorShift",
			true,
			seed => new XorShift (seed),
			gen => ((XorShift)gen).Next ());
		
		HashWrapper pcgHash = new HashWrapper (
			"PcgHash",
			false,
			seed => new PcgHash (),
			(gen, index) => ((PcgHash)gen).GetHash (index));
		
		HashWrapper md5 = new HashWrapper (
			"MD5",
			false,
			seed => MD5.Create (),
			(gen, index) => BitConverter.ToUInt32 (((MD5)gen).ComputeHash (BitConverter.GetBytes (index)), 0));
		
		HashWrapper murmur3 = new HashWrapper (
			"MurmurHash3",
			true,
			seed => new MurmurHash (seed),
			(gen, index) => ((MurmurHash)gen).GetHash (index));
		
		HashWrapper murmur3Array = new HashWrapper (
			"MurmurHash3 (array input)",
			true,
			seed => new MurmurHash (seed),
			(gen, index) => ((MurmurHash)gen).GetHash (new int[] {index}));
		
		HashWrapper xxHash = new HashWrapper (
			"xxHash",
			true,
			seed => new XXHash (seed),
			(gen, index) => ((XXHash)gen).GetHash (index));
		
		HashWrapper xxHashArray = new HashWrapper (
			"xxHash (array input)",
			true,
			seed => new XXHash (seed),
			(gen, index) => ((XXHash)gen).GetHash (new int[] {index}));
		
		HashWrapper wangHash = new HashWrapper (
			"WangHash",
			true,
			seed => new WangHash (seed),
			(gen, index) => ((WangHash)gen).GetHash (index));
		
		HashWrapper wangDoubleHash = new HashWrapper (
			"WangDoubleHash",
			true,
			seed => new WangDoubleHash (seed),
			(gen, index) => ((WangDoubleHash)gen).GetHash (index));
		
		// RNGs
		AddTest (new RNGSequence (systemRandom, 0));
		AddTest (new RNGSequence (xorShift, 1));
		
		// RNGs with varying seeds
		AddTest (new RNGVarySeedSequence (systemRandom, 0));
		AddTest (new RNGVarySeedSequence (systemRandom, 100));
		AddTest (new RNGVarySeedSequence (systemRandomScrambled, 0));
		
		// Hash functions
		AddTest (new HashSequence (pcgHash, 0));
		AddTest (new HashSequence (md5, 0));
		AddTest (new HashSequence (murmur3, 0));
		AddTest (new HashSequence (murmur3Array, 0));
		AddTest (new HashVarySeedSequence (murmur3, 0));
		AddTest (new HashSequence (xxHash, 0));
		AddTest (new HashSequence (xxHashArray, 0));
		AddTest (new HashVarySeedSequence (xxHash, 0));
		AddTest (new HashSequence (wangHash, 0));
		AddTest (new HashSequence (wangDoubleHash, 0));
		AddTest (new HashVarySeedSequence (wangDoubleHash, 0));
		
		// Dumb linear function for reference
		AddTest (new HashSequence (linear, 0));
	}

	private void AddTest (RandomSequence sequence) {
		tests.Add (new RandomnessTest (sequence));
	}
}

public class RNGWrapper {
	public delegate object GetGenerator (int seed);
	public delegate uint GetNext (object generator);
	public string name;
	public bool supportsSeed;
	public GetGenerator getGenerator;
	public GetNext getNext;
	public RNGWrapper (string name, bool supportsSeed, GetGenerator generator, GetNext next) {
		this.name = name;
		this.supportsSeed = supportsSeed;
		getGenerator = generator;
		getNext = next;
	}
}

public class HashWrapper {
	public delegate object GetGenerator (int seed);
	public delegate uint GetHash (object generator, int input);
	public string name;
	public bool supportsSeed;
	public GetGenerator getGenerator;
	public GetHash getHash;
	public HashWrapper (string name, bool supportsSeed, GetGenerator generator, GetHash hash) {
		this.name = name;
		this.supportsSeed = supportsSeed;
		getGenerator = generator;
		getHash = hash;
	}
}

public class RNGSequence : RandomSequence {
	private RNGWrapper wrapper;
	private int seed;
	private object generator;
	public RNGSequence (RNGWrapper wrapper, int seed) {
		this.wrapper = wrapper;
		this.seed = seed;
		Reset ();
	}
	public void Reset () {
		generator = wrapper.getGenerator (seed);
	}
	public uint Next () {
		return wrapper.getNext (generator);
	}
	public string GetName () {
		if (wrapper.supportsSeed)
			return string.Format ("{0}\nnumbers # of seed {1}", wrapper.name, seed);
		else
			return string.Format ("{0}\nnumbers #", wrapper.name);
	}
}

public class RNGVarySeedSequence : RandomSequence {
	private RNGWrapper wrapper;
	private int index;
	private int seed;
	public RNGVarySeedSequence (RNGWrapper wrapper, int index) {
		if (!wrapper.supportsSeed)
			throw new System.Exception ("RNGWrapper for RNGVarySeedSequence must support seed.");
		this.wrapper = wrapper;
		this.index = index;
		Reset ();
	}
	public void Reset () {
		seed = 0;
	}
	public uint Next () {
		object generator = wrapper.getGenerator (seed);
		seed++;
		// Get the number in the sequence corresponding to index.
		// This is super slow for large index values!
		for (int i=0; i<index; i++)
			wrapper.getNext (generator);
		return wrapper.getNext (generator);
	}
	public string GetName () {
		return string.Format ("{0}\n{1} number of seed #", wrapper.name, index.Ordinal ());
	}
}

public class HashSequence : RandomSequence {
	private HashWrapper wrapper;
	private int seed;
	private object generator;
	private int index;
	public HashSequence (HashWrapper wrapper, int seed) {
		this.wrapper = wrapper;
		this.seed = seed;
		generator = wrapper.getGenerator (seed);
		Reset ();
	}
	public void Reset () {
		index = 0;
	}
	public uint Next () {
		uint value = wrapper.getHash (generator, index);
		index++;
		return value;
	}
	public string GetName () {
		if (wrapper.supportsSeed)
			return string.Format ("{0}\nnumbers # of seed {1}", wrapper.name, seed);
		else
			return string.Format ("{0}\nnumbers #", wrapper.name);
	}
}

public class HashVarySeedSequence : RandomSequence {
	private HashWrapper wrapper;
	private int index;
	private int seed;
	public HashVarySeedSequence (HashWrapper wrapper, int index) {
		if (!wrapper.supportsSeed)
			throw new System.Exception ("HashWrapper for HashVarySeedSequence must support seed.");
		this.wrapper = wrapper;
		this.index = index;
		Reset ();
	}
	public void Reset () {
		seed = 0;
	}
	public uint Next () {
		uint value = wrapper.getHash (wrapper.getGenerator (seed), index);
		seed++;
		return value;
	}
	public string GetName () {
		return string.Format ("{0}\n{1} number of seed #", wrapper.name, index.Ordinal ());
	}
}
