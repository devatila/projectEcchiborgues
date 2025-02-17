using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimPlayer : MonoBehaviour
{
    public Animator anim;
    private Player_Movement playerMovement;
    private PlayerInventory inventory;

    private int startGunID;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();


        inventory.OnSwapWeapon += AnimSwitchGun;
        inventory.OnGetWeapon += AnimSwitchGun;

        if (inventory.gunEquipped != null)
        {
            startGunID = inventory.gunEquipped.GetComponent<Gun_Attributes>().gunId;
            
            anim.SetInteger("GunID", startGunID);
            anim.SetInteger("StateParam", 0);
        }
        playerMovement = GetComponent<Player_Movement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerMovement.MOVE_DIRECTION.sqrMagnitude != 0)
        {
            anim.SetInteger("StateParam", 1);          
        }
        else
        {
            anim.SetInteger("StateParam", 0);
        }
        
    }

    public void PlayShootAnimation()
    {
        anim.SetTrigger("IsShooting");
    }

    public void ReloadGun(float reloadTime)
    {
        float animationSpeed = 1.0f / reloadTime; 
        anim.SetFloat("FloatSpeed", animationSpeed);
        anim.SetTrigger("Reload");

    }

    public void AnimSwitchGun()
    {
        anim.SetInteger("GunID", inventory.gunEquipped.GetComponent<Gun_Attributes>().gunId);
        anim.SetTrigger("SwapGun");
    }

    public void AttachShotgun(int rechargeID, bool needRecharge)
    {
        anim.SetInteger("RechargeID", rechargeID);
        anim.SetBool("NeedRecharge?", needRecharge);
        
    }

    public void PlayDamageAnimation()
    {
        anim.SetTrigger("TakeDamage");
    }

    public void SwitchEnableAnimations()
    {
        anim.SetBool("CanTrasite", !anim.GetBool("CanTrasite"));
    }

    public void StopOrContinueAnimations(bool state)
    {
        anim.SetBool("CanTrasite", state);
    }
    
    
    public void PlayThrowableAnimation()
    {
        anim.SetTrigger("Throwing");
    }
}
