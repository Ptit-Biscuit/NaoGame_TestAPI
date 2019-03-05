using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour {

    public Level level;
    private XXHash hash;
    public List<Room> rooms;
    public List<Room> generatedRooms;
    private static System.Func<Room, SpawnPoint, bool> checkInverse =
        (room, spawnPoint) => room.spawnPoints.Any(sp => sp.orientation == inverseOrientation(spawnPoint.orientation));
    private static System.Func<Room, ETag, bool> checkTag = (room, tag) => room.tags.Contains(tag);
    private static int plop = 0;

    void Start() {
        if (level == null || level.startRoom == null) {
            return;
        }

        hash = new XXHash(level.seed);

        generatedRooms = new List<Room>();
        generatedRooms.Add(Instantiate(level.startRoom));

        StartCoroutine(SpawnNextRoom(generatedRooms.Last(), hash.GetHash(generatedRooms.Count())));
        // TODO: Spawn filler rooms
        // TODO: Check if level has [Shop, Loot, Combat, etc]
    }

    IEnumerator SpawnNextRoom(Room last, uint hashValue) {
        XXHash roomHash = new XXHash((int) hashValue);
        SpawnPoint spawnPoint = last.spawnPoints.ElementAt(roomHash.Range(0, last.spawnPoints.Count(), (int) hashValue));
        IEnumerable<Room> availableRooms =
        rooms.Where(room => checkInverse(room, spawnPoint) && !checkTag(room, ETag.END) && !checkTag(room, ETag.BLOCK_END));
        Room roomToSpawn = availableRooms.ElementAt(roomHash.Range(0, availableRooms.Count(), (int) hashValue));

        // if (plop < 10) {
        //     roomToSpawn = availableRooms.ElementAt(roomHash.Range(0, availableRooms.Count(), (int) hashValue));
        // } else {
        //     roomToSpawn = rooms.First(room => checkTag(room, ETag.BLOCK_END));
        //     plop = 0;
        // }

        Room spawnedRoom = Instantiate(roomToSpawn, Vector2.zero, roomToSpawn.transform.rotation);
        SpawnPoint otherSpawnPoint =
			spawnedRoom.spawnPoints.First(sp => sp.orientation == inverseOrientation(spawnPoint.orientation));

        spawnedRoom.transform.position = spawnPoint.transform.position + -otherSpawnPoint.transform.position;

        yield return new WaitForSeconds(0.05f);

        if (!spawnedRoom.overlaps) {
            last.DestroySpawnPoint(spawnPoint);
            spawnedRoom.DestroySpawnPoint(otherSpawnPoint);
            generatedRooms.Add(spawnedRoom);
        } else {
            Destroy(spawnedRoom.gameObject);
            plop++;
        }

        yield return generatedRooms.Count() < level.minimumRooms ?
            StartCoroutine(SpawnNextRoom(generatedRooms.Last(), hash.GetHash(hashValue))) : null;
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
