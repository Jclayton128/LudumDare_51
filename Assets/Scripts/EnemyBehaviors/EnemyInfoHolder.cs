using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoHolder : MonoBehaviour {
    public enum EnemyType {
        Sniper,
        Rat,
        Ogre,
        Mancubus,
        Type4,
        Type5,
    }

    public EnemyType enemyType;
}
