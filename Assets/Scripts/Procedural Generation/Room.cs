using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public int width;
	public int height;
	public List<ETag> tags;
	public List<SpawnPoint> spawnPoints;
	public bool overlaps = false;

	// TODO; remove later, it's just here to debug
	public Rect debugZone;

	public void DestroySpawnPoint(SpawnPoint spawnPoint) {
		spawnPoints.Remove(spawnPoint);
		Destroy(spawnPoint.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		overlaps = true;
	}

	// TODO: remove later
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		debugZone = new Rect();
		debugZone.size = new Vector2(width - 1, height - 1);
		Gizmos.DrawWireCube(transform.position, debugZone.size);
	}
}
