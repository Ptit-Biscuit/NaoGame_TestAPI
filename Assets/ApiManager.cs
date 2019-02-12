using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour {

	public Text text;
	
	public void Reset()
	{
		StartCoroutine(getReset());
	}

	IEnumerator getReset()
	{
		using (UnityWebRequest reset = new UnityWebRequest("http://51.15.121.74:3000/reset"))
		{
			yield return reset.SendWebRequest();
			// text.text = reset.text;
		}
	}

	public void Coucou()
	{
		StartCoroutine(getCoucou());
	}

	IEnumerator getCoucou()
	{
		using (UnityWebRequest coucou = new UnityWebRequest("http://51.15.121.74:3000/coucou"))
		{
			yield return coucou.SendWebRequest();
			text.text = coucou.text;
		}
	}

	public void Bonjour()
	{
		StartCoroutine(getBonjour());
	}

	IEnumerator getBonjour()
	{
		using (UnityWebRequest bonjour = new UnityWebRequest("http://51.15.121.74:3000/bonjour"))
		{
			yield return bonjour.SendWebRequest();
			// text.text = bonjour.text;
		}
	}
}
