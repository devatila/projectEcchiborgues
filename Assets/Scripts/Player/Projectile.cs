using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Pool_Projectiles pool;
    public int damage;
    public float lifetime;
    private GameObject gobj;

    [Space()]

    [SerializeField] private StatesPercentage states;

    void Start()
    {
        states = new StatesPercentage();
        pool = FindObjectOfType<Pool_Projectiles>();
        gobj = this.gameObject;
    }

    private void OnEnable()
    {
        Invoke("Deactivate", lifetime);
    }
    // Update is called once per frame
    void Deactivate()
    {
        if(gobj != null)
            pool.ReturnObject(gobj);
    }

    private void OnTriggerEnter2D(Collider2D collision) //Não esquecer de trocar para Physics2D.OverlapCircle
    {
        IDamageable dmg = collision.gameObject.GetComponent<IDamageable>();
        if (dmg != null && collision.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            dmg.TakeDamage(damage);
            CheckAndApplyStates(dmg);
        }

        IThrowable throwable = collision.GetComponent<IThrowable>();
        if(throwable != null)
        {
            throwable.OnHitObject();
        }
    }

    void CheckAndApplyStates(IDamageable dmg)
    {
        // Chance to apply Fire effect
        if (Random.value < states.FireEffectProbability)
        {
            dmg.ApplyNaturalState(NaturalStates.Fire, 5f);
        }
        // Chance to apply Stun (Electric) effect
        if (Random.value < states.StunEffectProbability)
        {
            dmg.ApplyNaturalState(NaturalStates.Eletric, 5f);
        }
        // Chance to apply Slow (Cold) effect
        if (Random.value < states.SlowEffectProbability)
        {
            // dmg.ApplyNaturalState(NaturalStates.Cold, slowDuration);
        }
    }

    public void GetStatesPercentages(StatesPercentage newStates)
    {
        states = newStates;
    }

    [System.Serializable]
    public class StatesPercentage
    {
        [Header("Effect Probabilities (0 to 1)")]
        [Range(0f, 1f)] public float FireEffectProbability = 0f;
        [Range(0f, 1f)] public float StunEffectProbability = 0f;
        [Range(0f, 1f)] public float SlowEffectProbability = 0f;
    }

}
