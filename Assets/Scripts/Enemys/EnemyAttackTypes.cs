using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAttackTypes
{
    public enum DogBot { Bite, Dash, TripleBite}
    public enum SoldierBot { Shoot, Punch }
}

[EnemyAttackEnum(EnemyBase.EnemyTypes.DogBot, typeof(EnemyAttackTypes.DogBot))]
public class DogBotAttackInfo { }

[EnemyAttackEnum(EnemyBase.EnemyTypes.SoldierBot, typeof(EnemyAttackTypes.SoldierBot))]
public class SoldierBotAttackInfo { }
