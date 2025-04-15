
using UnityEngine;

public class PlayerSpeedBoostPerk : PerkBase
{
    private float multiplier;
    private Player_Movement Player;
    private float defaultSpeed;
    public PlayerSpeedBoostPerk(Player_Movement Player, float multiplier)
    {
        this.multiplier = multiplier;
        this.Player = Player;
    }
    public override void OnApply()
    {
        Debug.Log("Perk de aumentar a velocidade acionado");
        defaultSpeed = Player.ACTUAL_SPEED;
        Player.ACTUAL_SPEED *= multiplier;
        
    }

    public override void OnRemove()
    {
        Player.ACTUAL_SPEED = defaultSpeed;
    }
}
