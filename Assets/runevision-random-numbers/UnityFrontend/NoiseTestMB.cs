/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TestFront {
	public bool enabled = true;
	public Texture2D noiseTexture;
	public Texture2D coordTexture;
}

public class NoiseTestMB : MonoBehaviour {
	RandomnessTester tester;
	TestFront[] fronts;
	Vector2 scroll;
	bool showOptions = false;
	
	const int width = 256;
	const int hSpacing = 20;
	const int vSpacing = 10;
	public Font font;
	GUIStyle header;
	GUIStyle style;
	bool showNoise = true;
	bool showCoords = true;
	bool showStats = true;
	
	void Start () {
		tester = new RandomnessTester ();
		fronts = new TestFront[tester.tests.Count];

		for (int i=0; i<fronts.Length; i++) {
			Initialize (i);
		}
	}

	void Initialize (int testNr) {
		RandomnessTest test = tester.tests[testNr];
		TestFront front = new TestFront ();
		fronts[testNr] = front;

		// Create noise texture.
		front.noiseTexture = new Texture2D (256, 256);
		front.noiseTexture.filterMode = FilterMode.Point;
		test.Reset ();
		for (int j=front.noiseTexture.height-1; j>=0; j--) {
			for (int i=0; i<front.noiseTexture.width; i++) {
				float f = test.noiseSequence[i + j*256];
				front.noiseTexture.SetPixel (i, j, new Color (f,f,f,1));
			}
		}
		front.noiseTexture.Apply ();

		// Create coords texture.
		front.coordTexture = new Texture2D (256, 256);
		front.coordTexture.filterMode = FilterMode.Point;
		for (int j=front.coordTexture.height-1; j>=0; j--) {
			for (int i=0; i<front.coordTexture.width; i++) {
				float f = Mathf.Min (1, test.coordsArray[i,j]);
				front.coordTexture.SetPixel (i, j, new Color (f,f,f,1));
			}
		}
		front.coordTexture.Apply ();
	}
	
	void OnGUI () {
		if (style == null) {
			style = new GUIStyle ("label");
			style.font = font;
			style.normal.textColor = Color.black;
			style.fontSize = 10;
			header = new GUIStyle ("label");
			header.normal.textColor = Color.black;
			header.fontSize = 12;
			header.fontStyle = FontStyle.Bold;
		}
		
		scroll = GUI.BeginScrollView (
			new Rect (0, 0, Screen.width, Screen.height-20),
			scroll,
			new Rect (0, 0, hSpacing + (width + hSpacing) * fronts.Count (e => e.enabled), 720));
			
		float left = hSpacing;
		float top = 10;
		for (int i=0; i<fronts.Length; i++) {
			TestFront front = fronts[i];
			if (!front.enabled)
				continue;

			RandomnessTest test = tester.tests[i];
			top = vSpacing;
			float height;

			height = 34;
			GUI.Label (new Rect (left, top, width, height), test.name, header);
			top += height + vSpacing;

			if (showNoise) {
				height = 256;
				GUI.DrawTexture (new Rect (left, top, 256, height), front.noiseTexture);
				top += height;

				height = 20;
				GUI.Label (new Rect (left, top, width, height), "Sequence of 65536 random values.", style);
				top += height + vSpacing;
			}

			if (showCoords) {
				height = 256;
				GUI.DrawTexture (new Rect (left, top, 256, height), front.coordTexture);
				top += height;

				height = 20;
				GUI.Label (new Rect (left, top, width, height), "Plot of 500000 random coordinates.", style);
				top += height + vSpacing;
			}

			if (showStats) {
				height = 96;
				GUI.Label (new Rect (left, top, width, height), test.result, style);
				top += height + vSpacing;
			}

			left += width + hSpacing;
		}
		
		GUI.EndScrollView ();

		if (GUI.Button (new Rect (0, Screen.height - 20, 80, 20), "Options"))
			showOptions = !showOptions;

		if (GUI.Button (new Rect (90, Screen.height - 20, 80, 20), "Screenshot"))
			StartCoroutine (CaptureScreenshot ((int)left, (int)top));

		if (showOptions)
			GUILayout.Window (0, new Rect (0, 0, Screen.width, Screen.height-20), OptionsWindow, "Options");
	}

	void OptionsWindow (int id) {
		for (int i = 0; i < fronts.Length; i++) {
			fronts[i].enabled = GUILayout.Toggle (fronts[i].enabled, tester.tests[i].name);
		}
		GUILayout.Space (10);
		showNoise = GUILayout.Toggle (showNoise, "Show Noise Image");
		showCoords = GUILayout.Toggle (showCoords, "Show Coordinates Plot");
		showStats = GUILayout.Toggle (showStats, "Show Statistics");
	}

	IEnumerator CaptureScreenshot (int width, int height) {
		yield return new WaitForEndOfFrame ();

		// Create a texture, RGB24 format
		var tex = new Texture2D (width, height, TextureFormat.RGB24, false);
		// Read screen contents into the texture
		tex.ReadPixels (new Rect (0, Screen.height - height, width, height), 0, 0);
		tex.Apply ();

		// Encode texture into PNG
		var bytes = tex.EncodeToPNG ();
		Destroy (tex);

		// Save PNG
		System.IO.File.WriteAllBytes (Application.dataPath + "/../Random.png", bytes);
	}
}
