using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticReveriePerk : PerkBase
{
    private bool activate;
    private PlayerPerkManager player;
    public BallisticReveriePerk (bool activate, PerkSO perkso, PlayerPerkManager player) : base(perkso)
    {
        this.activate = activate;
        this.player = player;
    }

    public override void OnApply()
    {
        player.SetBallisticReverie(activate);
    }

    public override void OnRemove()
    {
        player.SetBallisticReverie(false);
    }
}
