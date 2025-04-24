using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    #region - Identidade e Tipo do Inimigo -

    public enum EnemyTypes { DogBot, SoldierBot, ComedorDeKiev }
    public EnemyTypes currentTypeOfEnemy;

    #endregion

    #region - Atributos Básicos -

    [Header("Atributos Básicos")]

    [SerializeField] protected float speed;
    [SerializeField] protected int health;
    [SerializeField] protected int damageAmmount;
    [SerializeField] protected int cashToDrop;

    [SerializeField] protected bool enableDefaultBehavior = true;
    [SerializeField] protected bool useAttackRangeAsStoppingDistance = true;

    [SerializeField] protected float attackRange;
    [SerializeField] protected Vector2 attackRangeOffset;

    public bool isPlayerOnAttackRange { get; set; }

    protected EnemyBasicsAttributes EnemyBasics = new EnemyBasicsAttributes();

    #endregion

    #region - Posicionamento e Visibilidade -

    protected Vector2 ultimaPosicao;
    protected Transform playerPos;
    public Transform centralPosition;

    [SerializeField] protected string targetLayer = "VisibleBound";
    protected Collider2D VisibleBoundsCollider;

    protected SpriteRenderer[] spriteRenderers;

    private bool isVisible;
    private bool _isVisible
    {
        get => isVisible;
        set
        {
            if (isVisible != value)
            {
                isVisible = value;
                EnemyVisiblePartsHandler();
            }
        }
    }

    #endregion

    #region - Sistema de Ataques e Zonas -

    public EnemyAttackZone[] attackZones;
    private CircleCollider2D attackZone;

    protected EnemyAttack enemyAttack;
    [SerializeField] protected bool runtimeAbleAttack;
    protected bool attackAllowanceByProbability;
    protected float currentDOTtime;
    public bool isRunningAttack;

    protected List<EnemyAttackZone> activeZones = new List<EnemyAttackZone>();

    #endregion

    #region - Sistema de Estados -

    [SerializeField] protected NaturalStates currentState = NaturalStates.None;
    protected Coroutine stateCoroutine;
    private Coroutine subStateCoroutine;

    public virtual NaturalStates CurrentState
    {
        get => currentState;
        protected set
        {
            if (currentState != value)
            {
                currentState = value;
                OnStateChanged(currentState);
            }
        }
    }

    #endregion

    #region - Efeitos e Debug -

    protected List<EnemyEffect> activeEffects = new List<EnemyEffect>();
    [SerializeField] protected List<string> debugEffectNames = new List<string>();

    #endregion

    public virtual void Start()
    {
        EnemyBasics.GetReferences(this);
        EnemyBasics.agent.speed = speed;
        if(useAttackRangeAsStoppingDistance) EnemyBasics.agent.stoppingDistance = attackRange;
        ultimaPosicao = transform.position;

        playerPos = FindObjectOfType<PlayerInventory>().gameObject.transform;

        VisibleBoundsCollider = GetChildColliderWithLayer(gameObject, targetLayer);

        GetAllSprites();
        GetCentralPoint();
    }

    public virtual void Update()
    {
        AjustarDirecao();
        if (enableDefaultBehavior)
        {
            DefaultBehavior();
        }


        if (CanPerformAttack())
        {
            if (enemyAttack == null)
            {
                SelectAndExecuteAttack();
            }
            else
            {
                if (!enemyAttack.isRunning)
                {
                    SelectAndExecuteAttack();
                }
            }
        }

        UpdatingEffect();
    }

    #region - Attack Zones Notifications -
    public void PlayerEnteredZone(EnemyAttackZone zone)
    {
        if (!activeZones.Contains(zone))
        {
            activeZones.Add(zone);
            isPlayerOnAttackRange = true;
        }
    }

    public void PlayerExitedZone(EnemyAttackZone zone)
    {
        activeZones.Remove(zone);
        if (activeZones.Count == 0)
        {
            isPlayerOnAttackRange = false;
            if (enemyAttack != null)
            {
                enemyAttack.CancelAttacks();
                enemyAttack = null;
            }
        }
    }
    #endregion

    protected virtual void SelectAndExecuteAttack()
    {
        if (!isPlayerOnAttackRange || activeZones.Count == 0) return;
        if (enemyAttack != null && enemyAttack.isRunning == true) return;

        // Coleta todos os ataques possíveis
        List<EnemyAttackZone.AttackConfig> possibleAttacks = new List<EnemyAttackZone.AttackConfig>();
        foreach (var zone in activeZones)
        {
            possibleAttacks.AddRange(zone.GetAttacks());
        }

        if (possibleAttacks.Count == 0) return;

        // Seleciona um ataque com base na probabilidade
        var selectedAttack = SelectAttackBasedOnProbability(possibleAttacks);
        if (selectedAttack != null)
        {
            ExecuteAttack(selectedAttack);
        }
    }

    private EnemyAttackZone.AttackConfig SelectAttackBasedOnProbability(List<EnemyAttackZone.AttackConfig> attacks)
    {
        int totalProbability = attacks.Sum(a => a.probability);
        int randomValue = UnityEngine.Random.Range(0, totalProbability);
        int cumulative = 0;

        foreach (var attack in attacks)
        {
            cumulative += attack.probability;
            if (randomValue < cumulative)
            {
                return attack;
            }
        }
        return null;
    }

    protected virtual void ExecuteAttack(EnemyAttackZone.AttackConfig attackConfig)
    {
        // Subclasses devem implementar isso
        Debug.LogWarning($"Ataque {attackConfig.attackType} não implementado em {name}");
    }

    void UpdatingEffect()
    {
        if (activeEffects.Count == 0) return;
        
        float deltaTime = Time.deltaTime;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].UpdateEffect(deltaTime);
            if (activeEffects[i].isFinished)
                activeEffects.RemoveAt(i);
        }
        UpdateDebugEffectNames();
    }

    public virtual void SetGenericAttackType<T>(T attackType, int damage, int probability = 100) where T : Enum
    {
        //currentAttack = new DogBotAttacks(attackType, damage, this);
        Debug.Log($"Ataque selecionado: {attackType} causando {damage} de dano.");
        damageAmmount = damage;
    }

    public virtual void SetStun(bool hasToStun = true)
    {
        // Deixar o inimigo Stunnado
        if (hasToStun) Debug.Log("Imagina que o inimigo foi stunado aqui kkkk;-;");
        else Debug.Log(" O inimigo foi Desestunado");
    }

    public virtual void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        
    }

    public virtual void Move()
    {
        // Faça o inimigo se mover aqui
    }

    public virtual void Die()
    {

    }

    public virtual void Attack()
    {

    }
    public virtual void ApplyNaturalState(NaturalStates newState, float duration, float DOTtime = 1f)
    {
        currentDOTtime = DOTtime;

        if (stateCoroutine != null)
            StopCoroutine(stateCoroutine);

        CurrentState = newState;
        stateCoroutine = StartCoroutine(ClearStateAfterTime(duration));
    }

    public virtual void NewApplyNaturalState(NaturalStates stateType, float duration, float DOTtime = 1f)
    {
        currentDOTtime = DOTtime;

        EnemyEffect existingType = activeEffects.Find(e => e.stateType == stateType);
        if (existingType != null)
        {
            existingType.ResetDuration(duration);
            return;
        }

        EnemyEffect newEffect = null;

        switch (stateType)
        {
            case NaturalStates.Eletric:
                newEffect = new StunEffect(this, duration);
                break;
            case NaturalStates.Fire:
                newEffect = new FireEffect(this, duration);
                break;
        }

        if(newEffect != null)
        {
            newEffect.OnApply();
            activeEffects.Add(newEffect);
        }

        //UpdateDebugEffectNames(); // FUi ver a Champions

    }

    protected virtual IEnumerator ClearStateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        CurrentState = NaturalStates.None;
    }
    protected virtual void OnStateChanged(NaturalStates state)
    {
        CancelFireState();
        switch (state)
        {
            case NaturalStates.Eletric:
                SetStun(); // como o "SetStun"
                break;
            case NaturalStates.Fire:
                SetFireState(); // pode dar DOT (damage over time) - Vai ser DOT heheh
                break;
            case NaturalStates.Cold:
               //OnSlowStart(); // pode reduzir velocidade
                break;
            case NaturalStates.None:
                //OnEffectEnd();
                Debug.Log("Alterado Para Normal");
                break;
        }
    }

    public virtual void SetFireState()
    {
        if(subStateCoroutine == null)
        {
            subStateCoroutine = StartCoroutine(ApplyDOT(currentDOTtime));
        }
        else
        {
            StopCoroutine(subStateCoroutine);
            subStateCoroutine = null;
            subStateCoroutine = StartCoroutine(ApplyDOT(currentDOTtime));
            Debug.Log("Corrotina Resetada");
        }
        
    }
    public void CancelFireState()
    {
        if (subStateCoroutine != null)
        {
            StopCoroutine(subStateCoroutine);
            subStateCoroutine = null;
        }

        Debug.Log("Efeito de fogo Cancelado ou Encerrado");
    }
    private IEnumerator ApplyDOT(float time)
    {
        while (true)
        {
            Debug.Log("Tomando dano de fogo heheh"); 
            yield return new WaitForSeconds(time);
        }
    }

    public void UpdateDebugEffectNames()
    {
        debugEffectNames.Clear();
        foreach (var effect in activeEffects)
        {
            debugEffectNames.Add(effect.GetType().Name);
        }
    }


    private void DefaultBehavior() => EnemyBasics.agent.SetDestination(playerPos.position);

    public void AjustarDirecao()
    {
        _isVisible = IsVisibleOnCamera(VisibleBoundsCollider);
        if (!_isVisible) return; // Para o codigo se o inimigo não estiver aparecendo na camera

        // Se o inimigo estiver parado, verifica se precisa mudar a rotação
        if (IsEnemyStopped())
        {
            float indexRotation = EnemyBasics.agent.destination.x > EnemyBasics.agent.transform.position.x ? 0 : 180;

            if (transform.eulerAngles.y != indexRotation)
            {
                transform.rotation = Quaternion.Euler(0, indexRotation, 0);
            }
        }
        else
        {
            Vector2 direcaoMovimento = (Vector2)EnemyBasics.agent.transform.position - ultimaPosicao;

            if (Mathf.Abs(direcaoMovimento.x) > 0.001f) // Apenas atualiza se houver movimento
            {
                float novaRotacao = direcaoMovimento.x > 0 ? 0 : 180;

                if (transform.eulerAngles.y != novaRotacao)
                {
                    transform.rotation = Quaternion.Euler(0, novaRotacao, 0);
                }

                ultimaPosicao = EnemyBasics.agent.transform.position;
            }
        }
    }

    #region VisibleParts
    public void EnemyVisiblePartsHandler()
    {
        if (_isVisible)
        {
            Debug.Log("Apareceu na tela");
            ShowOnInScreen();
        }
        else
        {
            Debug.Log("Saiu da tela");
            HideOnOffScreen();
        }
    }

    private void HideOnOffScreen()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            sprite.enabled = false;
        }
    }

    private void ShowOnInScreen()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            sprite.enabled = true;
        }
    }
    #endregion
    private Collider2D GetChildColliderWithLayer(GameObject parent, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        foreach (Collider2D col in parent.GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject.layer == layer)
            {
                return col; // Retorna o primeiro Collider2D encontrado na layer correta
            }
        }

        return null; // Retorna nulo se nenhum Collider2D for encontrado na layer
    }

    private bool IsVisibleOnCamera(Collider2D objectTransform)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Collider2D col = objectTransform;

        if (col != null)
        {
            return GeometryUtility.TestPlanesAABB(frustumPlanes, col.bounds);
        }
        else
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(objectTransform.gameObject.transform.position);
            return viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;
        }
    }

    void GetAllSprites() => spriteRenderers = GetComponentsInChildren<SpriteRenderer>();


    public bool IsEnemyStopped()
    {
        return EnemyBasics.agent.remainingDistance <= EnemyBasics.agent.stoppingDistance && EnemyBasics.agent.velocity.magnitude < 0.1f;
    }

    void GetCentralPoint()
    {
        if (centralPosition != null) return;
        GameObject centroObj = transform.Find("CentralPoint").gameObject;

        if (centroObj != null && centroObj.transform.IsChildOf(transform))
        {
            centralPosition = centroObj.transform;
        }
        else
        {
            Debug.LogWarning($"PontoCentral não encontrado em {gameObject.name}, usando posição padrão.");
            centralPosition = transform;
        }
    }

    protected bool CanPerformAttack()
    {

        if (isPlayerOnAttackRange &&  runtimeAbleAttack)
        {
            return true;
        }
        return false;
    }

    public bool IsFacingPlayer()
    {
        Vector2 directionToPlayer = (EnemyBasics.agent.destination - transform.position).normalized;
        Vector2 facingDirection = transform.right; // Direção que o inimigo está olhando

        return Vector2.Dot(directionToPlayer, facingDirection) > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + attackRangeOffset.x, transform.position.y + attackRangeOffset.y, 0), attackRange);
    }

    [System.Serializable]
    public class EnemyBasicsAttributes
    {
        public NavMeshAgent agent;
        public Vector2 lastPosition;
        public Transform objectTransform;

        public void GetReferences(MonoBehaviour owner)
        {
            agent = owner.GetComponent<NavMeshAgent>();
            lastPosition = owner.transform.position;
            objectTransform = owner.transform;

            
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            
        }
    }
}

/*
 * 
[CustomEditor(typeof(EnemyBase), true)]
public class EnemyBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Refaz o inspector para não ter que repetir toda a configuração das variaveis base

        EnemyBase enemyBase = (EnemyBase)target;
        enemyBase.attackZones = enemyBase.GetComponentsInChildren<EnemyAttackZone>();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("== Attack Zones ==", EditorStyles.boldLabel);

        foreach (var zone in enemyBase.attackZones)
        {
            EditorGUILayout.BeginVertical("box");

            // Mostra o nome do GameObject com o AttackZone
            EditorGUILayout.LabelField("Trigger: " + zone.gameObject.name);

            // Mostra o nome do ataque selecionado
            EditorGUILayout.LabelField("Ataque: " + zone.selectedAttackName);

            // Campo de dano editável direto aqui
            zone.damage = EditorGUILayout.IntSlider("Dano", zone.damage, 0, 500);

            EditorGUILayout.EndVertical();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyBase);
            foreach (var zone in enemyBase.attackZones)
                EditorUtility.SetDirty(zone);
        }
    }
}
 * 
 * */