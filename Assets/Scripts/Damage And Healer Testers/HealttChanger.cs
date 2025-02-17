using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealttChanger : MonoBehaviour
{
    public enum stats
    {
        TakeDamage,
        GetHeal
    }
    public stats typeOfChanger;
    public int value;
    public float delay;

    [SerializeField]private bool isOnTrigger = false;
    [SerializeField]private bool canAct = true;
    private IPlayableCharacter playableChar;

    void Update()
    {
        if (isOnTrigger && canAct)
        {
            switch (typeOfChanger)
            {
                case stats.TakeDamage:
                    playableChar.TakeDamage(value);
                    StartCoroutine(timer(delay));
                    canAct = false;  
                    break;
                case stats.GetHeal:
                    playableChar.GetHeal(value);
                    StartCoroutine(timer(delay));
                    canAct = false;
                    break;

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playableChar = collision.GetComponent<IPlayableCharacter>();
        if (playableChar != null)
        {
            isOnTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isOnTrigger = false;
    }

    public IEnumerator timer(float timer)
    {
        yield return new WaitForSeconds(timer);
        canAct = true;
    }
}
