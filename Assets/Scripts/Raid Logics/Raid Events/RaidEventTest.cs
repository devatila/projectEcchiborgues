using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Raid Event", menuName = "Raid Event/Raid Event")]
public class RaidEventTest : RaidEventsClass
{
    public override void OnRaidStart()
    {
        Debug.Log("A raid Começou!");
    }

    public override void OnRaidEnd()
    {
        Debug.Log("A raid Terminou!");
    }
}
