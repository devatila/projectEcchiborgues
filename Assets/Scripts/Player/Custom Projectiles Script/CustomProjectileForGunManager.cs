using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomProjectileForGunManager : MonoBehaviour
{
    public int customID;
    public Transform correctPosition;

    private Player_Movement pMove;
    private bool facingRight;

    // Start is called before the first frame update
    void Start()
    {
        //bool alreadyExistPool = CustomPojectilePool.instance.CheckAlreadyExists(customID);

        CustomPojectilePool.instance.RegisterOnPool(GetComponent<Gun_Attributes>().projectile,ref customID);
        pMove = FindObjectOfType<Player_Movement>();
        
    }
    private void Update()
    {
        if (correctPosition == null) return;
        facingRight = pMove.facingRight;
        if (facingRight )
        {
            correctPosition.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            correctPosition.localRotation = Quaternion.Euler(0,180,0);
        }
    }
}
