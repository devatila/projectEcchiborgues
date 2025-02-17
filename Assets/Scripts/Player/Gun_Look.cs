using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Look : MonoBehaviour
{
    private bool GUN_FACING_RIGHT = true;
    private bool canMove = true;
    private void Start()
    {
        TypewriterEffectTMP.stopAll += StopLook;
        TypewriterEffectTMP.ContinueAll += KeepLook;
    }
    void Update()
    {
        if (canMove)
            GunLookMouseController();
    }

    private void GunLookMouseController()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        GunFlipController(mousePosition);
    }

    void GunFlipController(Vector3 mousePos)
    {
        if (mousePos.x < transform.position.x && GUN_FACING_RIGHT)
        {
            GunFlip();
        }
        else if (mousePos.x > transform.position.x && !GUN_FACING_RIGHT)
        {
            GunFlip();
        }
    }

    void GunFlip()
    {
        GUN_FACING_RIGHT = !GUN_FACING_RIGHT;
        transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y * -1, transform.localScale.z);
    }

    void StopLook()
    {
        canMove = false;
    }
    void KeepLook()
    {
        canMove = true;
    }
}
