
using UnityEngine;

public class PlayerSpeedBoostPerk : PerkBase
{
    private float multiplier;
    private Player_Movement Player;
    private float defaultSpeed;
    private bool isActive;
    public PlayerSpeedBoostPerk(Player_Movement Player, float multiplier)
    {
        this.multiplier = multiplier;
        this.Player = Player;
    }
    public override void OnApply()
    {
        isActive = true;
        Debug.Log("Perk de aumentar a velocidade acionado");
        defaultSpeed = Player.ACTUAL_SPEED;
        Player.ACTUAL_SPEED *= multiplier;
        
    }

    public override void OnRemove()
    {
        Player.ACTUAL_SPEED = defaultSpeed;
        isActive = false;
    }
    public override bool IsExpired => !isActive;
}
