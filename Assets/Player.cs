using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour {

	public int vitality = 100;
	
	public int defense = 100;

    [Range(30f, 100f)]
	public float intelligence = 30f;

    [Range(30f, 100f)]
	public float strength = 30f;

    [Range(20f, 100f)]
	public float agility = 20f;

    [Range(20f, 100f)]
	public float speed = 20f;

	public void reset() {
		this.vitality = 100;
		this.defense = 100;
		this.intelligence = 30f;
		this.strength = 30f;
		this.agility = 20f;
		this.speed = 20f;
	}
}
