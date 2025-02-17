using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeEffect : MonoBehaviour
{
    private PlayerInventory inventory;
    public Transform leftHandTransform;

    private GameObject lastObject;

    private bool previusState, currentState;

    private void Start()
    {
        inventory = transform.parent.GetComponent<PlayerInventory>();
    }

    public void PlaySoundAndRechargeEffect(AudioClip sfx)
    {
        inventory.gunEquipped.GetComponent<Gun_Shoot>().ShowCapsules();
    }

    public void GetRocketProjectile()
    {
        int id = inventory.gunEquipped.GetComponent<Gun_Attributes>().customID;
        lastObject = CustomPojectilePool.instance.GetObject(id);
        
        lastObject.transform.position = leftHandTransform.position;
        lastObject.transform.rotation = leftHandTransform.rotation;

        Transform playerObj = FindObjectOfType<Player_Movement>().playerObject.transform;
        if (playerObj.localScale.x < 0 && lastObject.transform.localScale.x > 0)
        {
            Debug.Log("Looking To Left");
            lastObject.transform.localScale = new Vector3(-lastObject.transform.localScale.x, lastObject.transform.localScale.y, lastObject.transform.localScale.z);
        }else if (playerObj.localScale.x > 0 && lastObject.transform.localScale.x < 0)
        {
            Debug.Log("Looking to Right");
            lastObject.transform.localScale = new Vector3(lastObject.transform.localScale.x * -1, lastObject.transform.localScale.y, lastObject.transform.localScale.z);

        }
        lastObject.transform.parent = leftHandTransform;
    }

    public void GetRocketRightPosition()
    {
        Transform t = inventory.gunEquipped.GetComponent<CustomProjectileForGunManager>().correctPosition;
        inventory.gunEquipped.GetComponent<Gun_Attributes>().projectile = lastObject;
        lastObject.transform.position = t.position;
        
        lastObject.transform.parent = t.parent;
        lastObject.transform.localRotation = Quaternion.identity;

        lastObject = null;
    }
}
