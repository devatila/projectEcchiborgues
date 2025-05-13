using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerkSO : ScriptableObject
{
    public string perkName;

    public abstract PerkBase CreatePerkInstance();
    public abstract Type GetPerkType();
}
