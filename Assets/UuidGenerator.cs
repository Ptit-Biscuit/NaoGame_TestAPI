using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UuidGenerator: MonoBehaviour {

	System.Guid guid;

	public Text text;

	public void newGuid() {
		DisplayPlayerInfo.playerUpdated = false;
		StartCoroutine(getGuid());
	}

	IEnumerator getGuid() {
		guid = System.Guid.NewGuid();
		text.text = guid.ToString();

		yield return guid;
	}
}
