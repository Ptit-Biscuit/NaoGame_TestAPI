using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "NaoGame_testAPI/Level", order = 0)]
public class Level : ScriptableObject {

    public int seed;

    public XXHash hash {
        get {
            return hash;
        }
        private set {
            hash = new XXHash(seed);
        }
    }

    [Range(5, 15)]
    public int minimumRooms = 5;

    public Room startRoom;

    // public bool hasShop = true;

    // public bool hasLoot = false;

    // public bool hasBoss = false;

    // public bool isFinalBoss = false;
}
