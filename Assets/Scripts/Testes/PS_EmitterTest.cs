using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PS_EmitterTest : MonoBehaviour
{
    public ParticleSystem pSystem;
    public bool isEquipped;

    public float effectProbability;
    // Start is called before the first frame update
    void Start()
    {
        pSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isEquipped)
        {
            pSystem.Play();
        }
        if (Input.GetMouseButtonUp(0) && !isEquipped)
        {
            pSystem.Stop();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(1);
        }
        IStateable stateable = other.GetComponent<IStateable>();
        if (stateable != null)
        {
            bool canEffect = (Random.Range(0f,1f) <= effectProbability) ? true : false;
            if (!canEffect) return;
            stateable.SetState(NaturalStates.Fire);
        }
    }

    public void OnShoot()
    {
        pSystem.Play();
    }
    public void OnUnshoot()
    {
        pSystem.Stop();
    }
    
}
