using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateable
{
    void SetState(NaturalStates State);
}
[System.Serializable]
public enum NaturalStates
{
    None,
    Fire,
    Cold,
    Eletric
}
public class EnemyNaturalStateManager : MonoBehaviour, IStateable
{
    [SerializeField]private NaturalStates _actualState;
    private NaturalStates actualState { get { return _actualState; } set {
        if(_actualState != value)
            {
                _actualState = value;
                OnChangeState();
            }
        } }
    public float probabilityToTakeState = 5;
    [Header("Damages")]
    public int fireStateDamage;
    public int eletricDamage;

    private IDamageable enemy;
    private Coroutine stateLoop, stateCountdown;
    private SpriteRenderer[] enemyParties;

    private int actualStateDamage;
    // Start is called before the first frame update
    private void Start()
    {
        actualState = NaturalStates.None;
        enemy = GetComponent<IDamageable>();
        enemyParties = GetComponentsInChildren<SpriteRenderer>();
    }
    #region FireStateSection
    public void SetFireState()
    {
        

        if (stateLoop != null) return;
        foreach (var enemy in enemyParties)
        {
            enemy.color = Color.red;
        }

        stateLoop = StartCoroutine(ActualStateLoop(1));
        stateCountdown = StartCoroutine(CountdownStates(5));
    }

    public void CancelFireState()
    {
        if (stateLoop == null) return;
        StopCoroutine(stateLoop);
        stateLoop = null;
        foreach (var enemy in enemyParties)
        {
            enemy.color = Color.white;
        }
        Debug.Log("O estado do Inimigo foi Alterado para: " + actualState);

    }

    IEnumerator ActualStateLoop(float damageTick)
    {
        Debug.Log("O estado do Inimigo foi Alterado para: " + actualState);
        while (true)
        {
            enemy.TakeDamage(fireStateDamage);
            
            yield return new WaitForSeconds(damageTick);

        }
    }
    #endregion
    
    #region EletricStateSection
    void SetEletricState()
    {

        if (stateLoop != null) return;
        foreach (var enemy in enemyParties)
        {
            enemy.color = Color.blue;
        }
        Debug.Log("Agora está Em EletricState");
        actualStateDamage = eletricDamage;
        stateLoop = StartCoroutine(ActualStateLoop(1));
        stateCountdown = StartCoroutine(CountdownStates(5));
    }


    #endregion

    public void SetState(NaturalStates State)
    {
        if (State == actualState) return;
        actualState = State;
    }
    void CancelAllStates()
    {
        CancelFireState();
        actualState = NaturalStates.None;
        _actualState = NaturalStates.None;
    }

    void OnChangeState()
    {
        
        switch (actualState)
        {
            case NaturalStates.None:
                CancelAllStates();
                break;
            case NaturalStates.Fire:
                SetFireState();
                break;
            case NaturalStates.Eletric:
                SetEletricState();
                break;
        }
    }

    IEnumerator CountdownStates(float timer)
    {
        yield return new WaitForSeconds(timer);
        actualState = NaturalStates.None;
        stateCountdown = null;
    }

    private void OnDisable()
    {
        CancelAllStates();
    }
}
/*
 *  float rng = Random.Range(0f, 1f);
        bool canSetState = rng < probabilityToTakeState * 10f;


        if (!canSetState)
        {
            actualState = NaturalStates.None;
            return;
        }

float rng = Random.Range(0f, 1f);
        bool canSetState = rng < probabilityToTakeState;
        
        
        if (!canSetState)
        {
            actualState = NaturalStates.None;
            return;
        }
 * 
 */