using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAttackTypes
{
    [System.Flags]
    public enum DogBot {
        Bite = 1 << 0,
        Dash = 1 << 1,
        TripleBite = 1 << 2,
    }
    public enum SoldierBot { Shoot, Punch }
}

[EnemyAttackEnum(EnemyBase.EnemyTypes.DogBot, typeof(EnemyAttackTypes.DogBot))]
public class DogBotAttackInfo { }

[EnemyAttackEnum(EnemyBase.EnemyTypes.SoldierBot, typeof(EnemyAttackTypes.SoldierBot))]
public class SoldierBotAttackInfo { }
