using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour {

	public int roomsWanted = 10;

	public Room startRoom;

	public List<Room> rooms;

	public List<Room> generatedRooms = new List<Room>();

	void Start() {
		if (startRoom == null) {
			startRoom = rooms[Random.Range(0, rooms.Count)];
		}

		generatedRooms.Add(Instantiate(startRoom));
	}

	public EOrientation inverseOrientation(EOrientation orientation) {
		switch(orientation) {
			default:
			case EOrientation.UP:
				return EOrientation.DOWN;
			case EOrientation.DOWN:
				return EOrientation.UP;
			case EOrientation.LEFT:
				return EOrientation.RIGHT;
			case EOrientation.RIGHT:
				return EOrientation.LEFT;
		}
	}
}
