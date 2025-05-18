using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpeedBoostPerk", menuName = "New Perk/New PlayerSpeedBoostPerk")]
public class SpeedBoostPerkSO : PerkSO
{

    public float multiplier;
    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        if(player == null)
        {
            Debug.LogError($"Componente {player.GetType().Name} não encontrado, Esta classe esta devidamente referenciada no objeto player?");
            return null;
        }
        return new PlayerSpeedBoostPerk(player, multiplier);
    }

    public override Type GetPerkType()
    {
        return typeof(PlayerSpeedBoostPerk);
    }
}
