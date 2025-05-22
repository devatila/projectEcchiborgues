using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
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

    [Space()]
    public bool canRegenerate = false;  // Se ativado, habilita o fator de regeneração do player
    public float regenAmount = 15f;     // Vida adicionada a cada tick
    public float regenInterval = 0.5f;    // Tempo entre cada tick
    private bool isRegenerationg = false;
    private Coroutine regenCoroutine;

    #region RegenerationCycle
    /// <summary>
    /// Altera os valores básicos do sistema de regeneração de vida
    /// </summary>
    /// <param name="newRegenAmount">Quanto de vida será adicionada em cada Tick</param>
    /// <param name="newRegenInterval">O tempo, em segundos, entre cada Tick ser executado</param>
    /// <param name="autoActivateRegeneration">Auto ativa o sistema de regeneração se estiver True</param>
    public void SetRegenerationValues(float newRegenAmount, float newRegenInterval = 1f, bool autoActivateRegeneration = true)
    {
        regenAmount = newRegenAmount;
        regenInterval = newRegenInterval;

        if (autoActivateRegeneration ) EnableRegeneration();
    }

    public void ResetRegenerationValues()
    {
        regenAmount = 15f;
        regenInterval = 0.5f;
    }
    public void EnableRegeneration()
    {
        canRegenerate = true;
        TryStartRegen();
    }

    public void DisableRegeneration()
    {
        canRegenerate = false;

        if(regenCoroutine != null) StopCoroutine(regenCoroutine);

        isRegenerationg = false;
    }

    public void TryStartRegen()
    {
        if (!canRegenerate || isRegenerationg || health >= maxHealth)
            return;

        if (regenCoroutine == null) regenCoroutine = StartCoroutine(RegenCoroutine());
        else
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = StartCoroutine(RegenCoroutine());
        }
    }

    private IEnumerator RegenCoroutine()
    {
        isRegenerationg = true;

        while (health < maxHealth && canRegenerate)
        {
            GetHeal(Mathf.RoundToInt(regenAmount));
            yield return new WaitForSeconds(regenInterval);
        }

        isRegenerationg = false;
        regenCoroutine = null;
    }

    #endregion

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
        if (health <= 0) return; // Não tem pq fazer o player tomar dano quando ele já esta m0rto ¯\_(ツ)_/¯

        int totalDamage = Mathf.RoundToInt(damageValue * damageMultiplier);
        if (armor > 0)
        {
            int diference = 0;
            if (totalDamage > armor)
            {
                diference = totalDamage - armor;
                Debug.Log("Passou do total da armadura, o dano repassado para a vida será de " + diference);
                DecreaseHealth(diference);
            }
            armor = Mathf.Max(0, armor - totalDamage);
        }
        else
        {
            DecreaseHealth(totalDamage);
        }
        playerAnim.PlayDamageAnimation();
        TryStartRegen();

    }
    
    void DecreaseHealth(int damageValue)
    {
        health -= Mathf.RoundToInt(damageValue);
        health = Mathf.Clamp(health, 0, maxHealth);
        lifeBar.fillAmount = (float)health / (float)maxHealth;
        if (lifeBar.fillAmount < 0.5f && CurrentLifeState == lifeStates.Fine)
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

    public void GetArmor(int armorValue)
    {
        armor += armorValue;
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


[CustomEditor(typeof(PlayerHealth))]
class PlayerHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerHealth ph = (PlayerHealth)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Activate Regeneration"))
        {
            ph.EnableRegeneration();
        }
    }
}

public interface IPlayableCharacter
{
    void TakeDamage(int damageValue);
    void GetHeal(int healValue);
}
