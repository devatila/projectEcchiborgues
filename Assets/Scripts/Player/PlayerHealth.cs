using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IPlayableCharacter
{
    public delegate void deathEvent();
    public event deathEvent OnDeath;

    public int health;
    private int maxHealth;
    public Image lifeBar;

    [Space()]

    public int armor;
    public int maxArmor;

    [Space()]
    public float damageMultiplier = 1;

    private GameObject hudHealth;
    private AnimPlayer playerAnim;

    public enum lifeStates
    {
        Fine,
        Caution,
        Danger
    }
    private lifeStates currentLifeState;
    public lifeStates CurrentLifeState { get { return currentLifeState; } set { 
        if(currentLifeState != value)
            {
                currentLifeState = value;
                OnStateChanged(currentLifeState);
            }
        
        } }

    void OnStateChanged(lifeStates newState)
    {
        switch (CurrentLifeState)
        {
            case lifeStates.Fine:
                StartCoroutine(ChangeColor(Color.green, 1f));
                break;
            case lifeStates.Caution:
                StartCoroutine(ChangeColor(Color.yellow, 1f));
                break;
            case lifeStates.Danger:
                StartCoroutine(ChangeColor(Color.red, 1f));
                break;
        }
    }

    private void Start()
    {
        maxHealth = health;
        lifeBar.fillAmount = (float)health / (float)maxHealth;
        hudHealth = lifeBar.transform.parent.parent.parent.gameObject;
        OnDeath += PlayerDead;
        playerAnim = GetComponent<AnimPlayer>();
    }

    public void TakeDamage(int damageValue)
    {
        health -= Mathf.RoundToInt(damageValue * damageMultiplier);
        health = Mathf.Clamp(health, 0, maxHealth);
        lifeBar.fillAmount = (float)health / (float)maxHealth;
        if(lifeBar.fillAmount < 0.5f && CurrentLifeState == lifeStates.Fine)
        {
            CurrentLifeState = lifeStates.Caution;
            Debug.Log(CurrentLifeState);
        } 
        else if (lifeBar.fillAmount < 0.25f && currentLifeState == lifeStates.Caution)
        {
            CurrentLifeState = lifeStates.Danger;
            Debug.Log(CurrentLifeState);
        }

        if (health <= 0)
        {
            OnDeath();
        }
        playerAnim.PlayDamageAnimation();
    }

    public void GetHeal(int healValue)
    {
        health += healValue;
        health = Mathf.Clamp(health, 0, maxHealth);
        lifeBar.fillAmount = (float)health / (float)maxHealth;
        if (lifeBar.fillAmount > 0.25f && CurrentLifeState == lifeStates.Danger)
        {
            CurrentLifeState = lifeStates.Caution;
            Debug.Log(CurrentLifeState);
        }
        else if (lifeBar.fillAmount > 0.5f && currentLifeState == lifeStates.Caution)
        {
            CurrentLifeState = lifeStates.Fine;
            Debug.Log(CurrentLifeState);
        }
    }

    public IEnumerator ChangeColor(Color endColor, float duration)
    {
        Color startColor = lifeBar.color;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            lifeBar.color = Color.Lerp(startColor, endColor, elapsedTime/ duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        lifeBar.color = endColor;
    }

    void LifeRegeneration()
    {
        // Lógica de regeneração de vida
    }

    void PlayerDead()
    {
        hudHealth.SetActive(false);
        TypewriterEffectTMP.stopAll();
    }
}

public interface IPlayableCharacter
{
    void TakeDamage(int damageValue);
    void GetHeal(int healValue);
}
