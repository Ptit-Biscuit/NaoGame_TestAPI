using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "NaoGame_testAPI/Level", order = 0)]
public class Level : ScriptableObject {

    public int seed;

    [Range(5, 15)]
    public int minimumRooms;

    public Room startRoom;

    // public bool hasShop = true;

    // public bool hasLoot = false;

    // public bool hasBoss = false;

    // public bool isFinalBoss = false;

    private void Awake() {
        seed = Random.Range(int.MinValue, int.MaxValue);
        minimumRooms = 5;
    }
}
