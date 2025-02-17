using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player_Effects : MonoBehaviour
{
    public SpriteRenderer muzzleSpriteRenderer;
    public Sprite[] muzzleSprites;
    public float muzzleLifetime = 0.02f;
    public float muzzleOffset = 0.6f;
    public Transform footPos;
    public SpriteRenderer muzzleFlashRenderer;
    public Light2D muzzleLight;

    private Player_Movement mMovement;
    private Vector3 mTransform;
    private Coroutine mCoroutine;

    private void Start()
    {
        muzzleSpriteRenderer.enabled = false;
        muzzleFlashRenderer.enabled = false;
        muzzleLight.enabled = false;
        mMovement = GetComponent<Player_Movement>();
        mTransform = muzzleSpriteRenderer.transform.localScale;
    }
    public void showMuzzle()
    {
        Gun_Attributes gun_Attributes = GetComponent<PlayerInventory>().gunEquipped.GetComponent<Gun_Attributes>();
        Transform shootpos = gun_Attributes.shootPosition;

        muzzleSpriteRenderer.transform.rotation = shootpos.rotation;
        if (mMovement.facingRight)
        {
            muzzleSpriteRenderer.transform.position = shootpos.position + shootpos.rotation * new Vector3(muzzleOffset, 0, 0);
            muzzleSpriteRenderer.transform.localScale = mTransform;
        }
        else
        {
            muzzleSpriteRenderer.transform.position = shootpos.position + shootpos.rotation * new Vector3(muzzleOffset * -1, 0, 0);
            muzzleSpriteRenderer.transform.localScale = new Vector3(mTransform.x * -1, mTransform.y, mTransform.z);
        }
        muzzleFlashRenderer.transform.position = muzzleSpriteRenderer.transform.position;

        
        if (mCoroutine != null)
        {
            StopAllCoroutines();
            mCoroutine = null;
        }
        else
        {
            TurnEffectsOnOrOff(true);
        }
        muzzleSpriteRenderer.sprite = muzzleSprites[Random.Range(0, muzzleSprites.Length)];
        mCoroutine = StartCoroutine(Countdown(muzzleLifetime));
    }

    private void TurnEffectsOnOrOff(bool desiredState)
    {
        muzzleSpriteRenderer.enabled = desiredState;
        muzzleFlashRenderer.enabled = desiredState;
        muzzleLight.enabled = desiredState;
    }

    IEnumerator Countdown(float time)
    {
        yield return new WaitForSeconds(time);
        TurnEffectsOnOrOff(false);
        mCoroutine = null;
    }
}
