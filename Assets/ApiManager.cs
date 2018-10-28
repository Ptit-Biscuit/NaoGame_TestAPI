using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApiManager : MonoBehaviour {

	public Text text;
	
	public void Reset()
	{
		StartCoroutine(getReset());
	}

	IEnumerator getReset()
	{
		using (WWW reset = new WWW("http://51.15.121.74:3000/reset"))
		{
			yield return reset;
			text.text = reset.text;
		}
	}

	public void Coucou()
	{
		StartCoroutine(getCoucou());
	}

	IEnumerator getCoucou()
	{
		using (WWW coucou = new WWW("http://51.15.121.74:3000/coucou"))
		{
			yield return coucou;
			text.text = coucou.text;
		}
	}

	public void Bonjour()
	{
		StartCoroutine(getBonjour());
	}

	IEnumerator getBonjour()
	{
		using (WWW bonjour = new WWW("http://51.15.121.74:3000/bonjour"))
		{
			yield return bonjour;
			text.text = bonjour.text;
		}
	}
}
