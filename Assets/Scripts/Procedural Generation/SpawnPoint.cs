using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public EOrientation orientation = EOrientation.UP;
    private Room roomToSpawn;
    private Vector3 position;

    public void Plop(Room room) {
        ProceduralGeneration procedural = GameObject.FindObjectOfType<ProceduralGeneration>();
        EOrientation inverse = procedural.inverseOrientation(orientation);

        StartCoroutine(
            CheckOverlap(procedural, inverse, canSpawn => {
                if (canSpawn) {
                    Room spawnedRoom = Instantiate(roomToSpawn, position, roomToSpawn.transform.rotation);

                    Destroy(spawnedRoom.spawnPoints.First(sp => sp.orientation == inverse).gameObject);
                    Destroy(gameObject);
                    procedural.generatedRooms.Add(spawnedRoom);
                } else {
                    Plop(room);
                }
            })
        );
    }

    IEnumerator CheckOverlap(ProceduralGeneration procedural, EOrientation inverse, System.Action<bool> canSpawn) {
        IEnumerable<Room> availableRooms = procedural.rooms.Where(availableRoom =>
            !procedural.generatedRooms.Contains(availableRoom) &&
            availableRoom.spawnPoints.Any(sp => sp.orientation == inverse)
        );

        roomToSpawn = availableRooms.ElementAt(Random.Range(0, availableRooms.Count()));
        position = transform.position
                    + (roomToSpawn.transform.position
                        - roomToSpawn.spawnPoints.First(sp => sp.orientation == inverse).transform.position);

        roomToSpawn.checkZone.center = position;

        bool overlaps = false;
        procedural.generatedRooms.ForEach(room => {
            if (room.checkZone.Overlaps(roomToSpawn.checkZone)) {
                overlaps = true;
            }
        });

        yield return !overlaps;

        canSpawn(!overlaps);
    }
}