using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public int width;
	public int height;
	public List<SpawnPoint> spawnPoints;

	public Rect checkZone; 

	void Start() {
		checkZone = new Rect();
		checkZone.size = new Vector2(width - 1, height - 1);
		checkZone.center = transform.position;

		foreach(SpawnPoint spawnPoint in spawnPoints) {
			spawnPoint.Plop(this);
		}
	}
}
