/* 
 * This source code file is in the public domain.
 * Permission to use, copy, modify, and distribute this software and its documentation
 * for any purpose and without fee is hereby granted, without any conditions or restrictions.
 * This software is provided “as is” without express or implied warranty.
 * 
 * Original code by Rune Skovbo Johansen based on code snippet by Adam Smith.
 */

using System;

public class PcgHash : HashFunction {
	
	public override uint GetHash (params int[] data) {
		if (data.Length >= 2)
			return GetHash (data[0], data[1]);
		else
			return GetHash (data[0]);
	}
	
	public override uint GetHash (int i, int j) {
		int a = i;
		int b = j;
		for (int r = 0; r < 3; r++) {
			a = rot ((int) ((a^0xcafebabe) + (b^0xfaceb00c)), 23);
			b = rot ((int) ((a^0xdeadbeef) + (b^0x8badf00d)), 5);
		}
		return (uint)(a^b);
	}
	
	public override uint GetHash (int i) {
		int a = i;
		int b = 0;
		for (int r = 0; r < 3; r++) {
			a = rot ((int) ((a^0xcafebabe) + (b^0xfaceb00c)), 23);
			b = rot ((int) ((a^0xdeadbeef) + (b^0x8badf00d)), 5);
		}
		return (uint)(a^b);
	}
	
	private static int rot (int x, int b) {
		return (x << b) ^ (x >> (32-b)); // broken rotate because I'm in java and lazy
	}
}
