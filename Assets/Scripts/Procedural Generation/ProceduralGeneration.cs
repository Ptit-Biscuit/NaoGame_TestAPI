using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour {

    public Level level;
    public List<Room> rooms;
    private List<Room> generatedRooms;

    private static System.Func<Room, SpawnPoint, bool> checkInverse =
        (room, spawnPoint) => room.spawnPoints.Any(sp => sp.orientation == inverseOrientation(spawnPoint.orientation));
    private static System.Func<Room, ETag, bool> checkTag = (room, tag) => room.tags.Contains(tag);

    void Start() {
        if (level == null || level.startRoom == null) {
            return;
        }

		generatedRooms = new List<Room>();
        generatedRooms.Add(Instantiate(level.startRoom));
        StartCoroutine(Generation());
    }

    IEnumerator Generation() {
		System.DateTime start = System.DateTime.Now;

        while (generatedRooms.Count <= level.minimumRooms) {
            StartCoroutine(SpawnNextRoom(generatedRooms.Count, generatedRooms.Last()));
            yield return new WaitForSeconds(0.2f);
        }

		Debug.Log("Done in " + (System.DateTime.Now - start));
    }

    public IEnumerator SpawnNextRoom(int roomIndex, Room last) {
        SpawnPoint spawnPoint = last.spawnPoints.ElementAt(Random.Range(0, last.spawnPoints.Count()));

        IEnumerable<Room> availableRooms = GetAvailableRooms(roomIndex, spawnPoint);
        Room roomToSpawn = availableRooms.ElementAt(Random.Range(0, availableRooms.Count()));

        Room spawnedRoom = Instantiate(roomToSpawn, Vector2.zero, roomToSpawn.transform.rotation);
        SpawnPoint otherSpawnPoint =
			spawnedRoom.spawnPoints.Where(sp => sp.orientation == inverseOrientation(spawnPoint.orientation)).First();

        spawnedRoom.transform.position = spawnPoint.transform.position + -otherSpawnPoint.transform.position;

        yield return new WaitForSeconds(0.1f);

        if (!spawnedRoom.overlaps) {
            last.DestroySpawnPoint(spawnPoint);
            spawnedRoom.DestroySpawnPoint(otherSpawnPoint);
            generatedRooms.Add(spawnedRoom);
        } else {
            Destroy(spawnedRoom.gameObject);
        }
    }

    public IEnumerable<Room> GetAvailableRooms(int roomIndex, SpawnPoint spawnPoint) {
        return rooms.Where(room => checkInverse(room, spawnPoint) &&
            (roomIndex < level.minimumRooms) ? !checkTag(room, ETag.END) : checkTag(room, ETag.END));
    }

    public static EOrientation inverseOrientation(EOrientation orientation) {
        switch (orientation) {
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
