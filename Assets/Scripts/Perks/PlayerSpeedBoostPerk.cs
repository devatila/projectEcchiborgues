
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpeedBoostPerk : PerkBase
{
    private float multiplier;
    private PlayerPerkManager Player;
    private float defaultSpeed;
    public PlayerSpeedBoostPerk(PerkSO perkso, PlayerPerkManager Player, float multiplier) : base(perkso)
    {
        this.multiplier = multiplier;
        this.Player = Player;
    }
    public override void OnApply()
    {
        isActive = true;
        Debug.Log("Perk de aumentar a velocidade acionado");
        Player.SetMovementMultiplier(multiplier);
        
    }

    public override void OnRemove()
    {
        Player.SetMovementMultiplier(-multiplier);
        isActive = false;
    }
}
