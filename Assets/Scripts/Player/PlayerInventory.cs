using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerInventory : MonoBehaviour // Todos devem TEMER este código
{
    public delegate void StopReloadOnSwitch();
    public event StopReloadOnSwitch OnStopRelaod;

    public delegate void Reloaded(int mag, int ammoValue);
    public event Reloaded OnReloaded;

    public delegate void SwapingWeapon();
    public event SwapingWeapon OnSwapWeapon;

    public event Action OnCanSwapWeapon;

    public delegate void GetWeapon();
    public event GetWeapon OnGetWeapon;

    public event Action<int> OnChangeCash;


    [Header("Arma Equipada")]
    public int numeroMaximoDeArmasEmMao = 2;

    public enum ammoTypeOfGunEquipped
    {
        AR,
        Sub,
        Shotgun,
        Pistol,
        Granade_Launcher,
        Rocket_Launcher,
        FlameThrower
    }
    public ammoTypeOfGunEquipped AmmoType;

    public GameObject gunEquipped;

    public int ARammount, subAmmount, shotgunAmmount, pistolAmmount, granadeLauncherAmmount, rocketLauncherAmmount, flamethrowerAmmount;

    [Header("Organizador de Arma")]
    public int weaponIndex;
    public GameObject[] objectsFromSingleton;
    public List<GameObject> weaponsOnHold = new List<GameObject> ();


    public float CollectRange;
    public LayerMask CollectableLayer;

    public bool canSwitchWeapon;
    public Transform gunPosition;

    public GameObject gunWindowAttributes;

    [Header("Itens de Inventário")]
    public int metalCash;

    private bool facingRight;
    [SerializeField]private AnimPlayer animPlayer;
    private bool isVisible;

    private UI_BuySystem buySystem;
    private bool isBuilding = false;
    private bool _isBuilding {
        get {return isBuilding; } 
        set { if (isBuilding != value && value == true) { ShowBuidingSet(); } isBuilding = value; } 
    }
    private GameObject objectToTransport;

    private Player_Movement pMovement;
    private AnimPlayer pAnim;
    public MouseTrackerForNewPlayer pTracker;

    public Transform dropPosition;

    private bool canCollect = false;
    [SerializeField] private bool isCollected;
    public Gun_Attributes equippedGunAttributes;
    public PlayerGunMultipliers playerGunMultipliers { get; set; }
    public Dictionary<ammoTypeOfGunEquipped, Func<float>> GunsMultipliers;
    public float GeneralGunsDamageMultiplier = 1f;
    public float GeneralGunsSpreadMultiplier = 1f;

    public Projectile.StatesPercentage GeneralStatesBulletPercentages { get; set; }

    public class AmmoMultipliers
    {
        public Func<float> Damage;
        public Func<float> FireRate;
        public Func<float> Spread;
        public Func<float> ReloadSpeed;
    }

    // 1) mapa arma → struct de multiplicadores
    private Dictionary<ammoTypeOfGunEquipped, PlayerGunMultipliers.GunMultipliers> gunStructMap;

    // 2) mapa arma → lambdas de multiplicadores
    private Dictionary<ammoTypeOfGunEquipped, AmmoMultipliers> multipliersByAmmo;

    public bool isEnableBallisticReverie;



    // Start is called before the first frame update
    void Awake()
    {
        playerGunMultipliers = new PlayerGunMultipliers();
        playerGunMultipliers.ResetValues();
        canSwitchWeapon = true;
        isVisible = false;
        animPlayer = GetComponent<AnimPlayer>();

        try
        {
            objectsFromSingleton = GunsInfosBetweenScenes.instance.ImaginaUmaArmaAqui?.ToArray();
        }
        catch
        {
            Debug.Log("Não tem Singleton aqui");
            if(objectsFromSingleton.Length == 0) objectsFromSingleton = null;
        }

        // Verificação de null e tamanho
        if (objectsFromSingleton != null && objectsFromSingleton.Length > 0)
        {
            //Debug.Log((objectsFromSingleton.Length > 1 ? objectsFromSingleton[1] == null : "Sem segundo objeto"));

            for (int i = 0; i < objectsFromSingleton.Length; i++)
            {
                if (objectsFromSingleton[i] != null)
                {
                    GameObject instantiatedObject = Instantiate(objectsFromSingleton[i]) as GameObject;
                    if (instantiatedObject != null)
                    {
                        
                        GetOrChangeWeaponSelected(instantiatedObject, false);
                    }
                    else
                    {
                        Debug.LogError($"Falha ao instanciar o objeto no índice {i}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Objeto no índice {i} é null");
                }
            }
        }
        else
        {
            Debug.LogWarning("Array objectsFromSingleton é null ou está vazio");
        }

        // Inicializa gunStructMap a partir de playerGunMultipliers
        BuildGunStructMap();
        // Gera multipliersByAmmo a partir do struct map
        BuildMultipliersMap();

    }

    private void Start()
    {
        GeneralStatesBulletPercentages = new Projectile.StatesPercentage();
        canCollect = false;
        TryGetBuySystem();

        gunWindowAttributes.SetActive(false);
        pMovement = GetComponent<Player_Movement>();
    }

    private void TryGetBuySystem()
    {
        if (buySystem != null) return;
        buySystem = FindObjectOfType<UI_BuySystem>();
        if (buySystem == null) return;
        buySystem.OnBuyItem += BuyingTraps;
    }

    public event Action OnBuildPlaced;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canSwitchWeapon)
        {
            SwitchWeapon();
        }

        if (canCollect && Input.GetKeyDown(KeyCode.F))
        {
            currentCollectable.Collect();
            canCollect = false;
            if(currentCollectable != null)
            {
                currentCollectable.ShowInfos(gunWindowAttributes, gunEquipped?.GetComponent<Gun_Attributes>());
                canCollect = true;
            }
        }

        if (_isBuilding)
        {
            objectToTransport.transform.position = dropPosition.position;
            if (Input.GetKeyDown(KeyCode.F) && objectToTransport.GetComponent<PlaceableObject>()._canBePlaced)
            {
                objectToTransport.GetComponent<PlaceableObject>().SetColorOnPlaced();
                objectToTransport = null;
                OnBuildPlaced?.Invoke();
                _isBuilding = false;
            }
        }
        //gunEquipped.transform.position = new Vector3( gunPosition.position.x + gunEquipped.GetComponent<Gun_Attributes>().offsetX, gunPosition.position.y + gunEquipped.GetComponent<Gun_Attributes>().offsetY, 0);
    }

    private void FixedUpdate()
    {
        CheckObjectsInRange();
    }

    #region AllAboutGun

    public void ReloadDesiredGun(int Magazine)
    {
        Debug.Log("Recarregou!");
        Gun_Attributes gun = gunEquipped.GetComponent<Gun_Attributes>();
        int maxMag = gun.magazine;
        int faltaParaPenteCheio = maxMag - gun.actual_magazine;

        Debug.Log(maxMag);

        if (gun.typeOfAmmo != AmmoType)
            return; // Se o tipo de munição da arma não for o mesmo do jogador, não recarrega

        // Determina qual variável de munição usar
        ref int ammoAmount = ref GetAmmoRef(gun.typeOfAmmo);

        // Recarrega a arma
        SetReloadValues(gun, ref ammoAmount, faltaParaPenteCheio);

        // Atualiza o HUD
        OnReloaded?.Invoke(gun.actual_magazine, ammoAmount);
    }

    // Método auxiliar para retornar a referência correta da munição
    private ref int GetAmmoRef(ammoTypeOfGunEquipped type)
    {
        switch (type)
        {
            case ammoTypeOfGunEquipped.AR:
                return ref ARammount;
            case ammoTypeOfGunEquipped.Sub:
                return ref subAmmount;
            case ammoTypeOfGunEquipped.Shotgun:
                return ref shotgunAmmount;
            case ammoTypeOfGunEquipped.Pistol:
                return ref pistolAmmount;
            case ammoTypeOfGunEquipped.Granade_Launcher:
                return ref granadeLauncherAmmount;
            case ammoTypeOfGunEquipped.Rocket_Launcher:
                return ref rocketLauncherAmmount;
            case ammoTypeOfGunEquipped.FlameThrower:
                return ref flamethrowerAmmount;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), $"Tipo de munição desconhecido: {type}");
        }
    }

    public void SetGunMultipliersByTypeOfGun(PlayerGunMultipliers multipliers, Projectile.StatesPercentage projEffects)
    {
        playerGunMultipliers = multipliers;
        GeneralGunsDamageMultiplier = playerGunMultipliers.allGunsMultiplier;
        GeneralGunsSpreadMultiplier = playerGunMultipliers.allGunsSpreadMultiplier;

        GeneralStatesBulletPercentages = projEffects;
        Debug.Log(equippedGunAttributes == null);

        if (equippedGunAttributes == null) return;

        equippedGunAttributes.isEnableBallisticReverie = isEnableBallisticReverie;

        // 1) Atualiza o struct map
        BuildGunStructMap();
        // 2) Recria as lambdas
        BuildMultipliersMap();

        // Aplica se houver uma arma equipada
        Debug.Log($"Stats Antes: dmg = {equippedGunAttributes.gunDamage}, rld = {equippedGunAttributes.reloadTime}, sprd = {equippedGunAttributes.maxSpread}, frt = {equippedGunAttributes.cadency}");
        float allGunsM = playerGunMultipliers.allGunsMultiplier;
        float damageM = multipliersByAmmo[AmmoType].Damage();

        if (equippedGunAttributes.weaponDataSO == null)
        {
            Debug.LogWarning("O prefab dessa arma não possui um WeaponSO referenciado. Ajustar isso o quanto antes");
            return;
        }

        //Area de dano
        ApplyStats(
            am => am.Damage(),
            (attr, v) => attr.gunDamage = v,
            attr => attr.weaponDataSO.gunDamage,
            GeneralGunsDamageMultiplier,
            roundResult: true
            );

        //Area de Reload
        ApplyStats(
            am => am.ReloadSpeed(),
            (attr, v) => attr.reloadTime = v,
            attr => attr.weaponDataSO.reloadTime,
            1f
            );

        //Area de Spread
        ApplyStats(
            am => am.Spread(),
            (attr, v) => attr.maxSpread = v,
            attr => attr.weaponDataSO.maxSpread,
            GeneralGunsSpreadMultiplier
            );

        //Area de Firerate
        ApplyStats(
            am => am.FireRate(),
            (attr, v) => attr.cadency = v,
            attr => attr.weaponDataSO.cadency,
            1f
            );
        // ...
        equippedGunAttributes.SetProjectileStates(GeneralStatesBulletPercentages);

        Debug.Log($"Stats Depois: dmg = {equippedGunAttributes.gunDamage}, rld = {equippedGunAttributes.reloadTime}, sprd = {equippedGunAttributes.maxSpread}, frt = {equippedGunAttributes.cadency}");

    }
    void OldWay()
    {
        /*
        // Eu chamo isso de RESOLVEDOR DE PROBLEMAS // Area de Dano
        equippedGunAttributes.gunDamage = Mathf.RoundToInt(equippedGunAttributes.weaponDataSO.gunDamage * damageM * allGunsM);
        //Debug.Log($"O dano causado pela arma:{equippedGunAttributes.typeOfAmmo} AGORA é de {equippedGunAttributes.gunDamage}");

        // Area de ReloadTime
        float reloadM = multipliersByAmmo[AmmoType].ReloadSpeed();
        equippedGunAttributes.reloadTime = equippedGunAttributes.weaponDataSO.reloadTime * reloadM;

        // Area de Spread
        float spreadM = multipliersByAmmo[AmmoType].Spread();
        equippedGunAttributes.maxSpread = equippedGunAttributes.weaponDataSO.maxSpread * spreadM;

        // Area de Firerate
        float firerateM = multipliersByAmmo[AmmoType].FireRate();
        equippedGunAttributes.cadency = equippedGunAttributes.weaponDataSO.cadency * firerateM; // Nesse caso. Quanto menor < 1 (= 100%), maior é o firerate
        */
    }
    private void ApplyStats<T>(
        Func<AmmoMultipliers, float> getter,
        Action<Gun_Attributes, T> setter,
        Func<Gun_Attributes, T> originalValueProvider,
        float allGunsM,
        bool roundResult = false
        )
    {
        float m = getter(multipliersByAmmo[AmmoType]);
        float original = Convert.ToSingle(originalValueProvider(equippedGunAttributes));
        float result = original * m * allGunsM; //(roundResult ? allGunsM : 1f);

        if (roundResult)
        {
            setter(equippedGunAttributes, (T)(object)Mathf.RoundToInt(result));
        }else
        {
            setter(equippedGunAttributes, (T)(object)result);
        }
    }

    private void BuildGunStructMap()
    {
        gunStructMap = new Dictionary<ammoTypeOfGunEquipped, PlayerGunMultipliers.GunMultipliers>
        {
            { ammoTypeOfGunEquipped.AR,                playerGunMultipliers.ArMultipliers },
            { ammoTypeOfGunEquipped.Sub,               playerGunMultipliers.SubMultipliers },
            { ammoTypeOfGunEquipped.Shotgun,           playerGunMultipliers.ShotgunMultipliers },
            { ammoTypeOfGunEquipped.Pistol,            playerGunMultipliers.PistolMultipliers },
            { ammoTypeOfGunEquipped.Granade_Launcher,  playerGunMultipliers.GranadeLauncherMultipliers },
            { ammoTypeOfGunEquipped.FlameThrower,      playerGunMultipliers.FlameThrowerMultipliers },
            { ammoTypeOfGunEquipped.Rocket_Launcher,   playerGunMultipliers.RocketLauncherMultipliers },
        };

    }

    private void BuildMultipliersMap()
    {
        multipliersByAmmo = gunStructMap.ToDictionary(
            kvp => kvp.Key,
            kvp => new AmmoMultipliers
            {
                Damage = () => kvp.Value.damageMultiplier,
                FireRate = () => kvp.Value.firerateMultiplier,
                ReloadSpeed = () => kvp.Value.reloadTimeMultiplier,
                Spread = () => kvp.Value.spreadMultiplier
            }
        );
    }


    public void ResetGunEquippedGunAttributes()
    {
        Debug.Log("Resetou");
        equippedGunAttributes.GetSOdata(equippedGunAttributes.weaponDataSO);
        Debug.Log($"Dano da arma equipada resetada para {equippedGunAttributes.gunDamage}");
    }

    void SetReloadValues(Gun_Attributes gunDesired, ref int desiredTypeAmmount, int ammountToReload)
    {
        int quantidadeParaRecarregar = Mathf.Min(desiredTypeAmmount, ammountToReload); // Pega o mínimo entre o necessário e o disponível

        gunDesired.actual_magazine += quantidadeParaRecarregar; // Adiciona a munição ao pente
        desiredTypeAmmount -= quantidadeParaRecarregar; // Remove a munição usada do total disponível
    }


    #region SwitchGunProccess
    public void SwitchWeapon()
    {
        if (weaponsOnHold.Count <= 1) return;

        if(OnStopRelaod != null && gunEquipped.GetComponent<Gun_Attributes>().isReloading == true)
        {
            OnStopRelaod(); //Agora que vi que escrevi errado kkkkkkkk
        }

        //
        // Incrementa o índice da arma
        weaponIndex = (weaponIndex + 1) % weaponsOnHold.Count;
        Debug.Log(weaponIndex);

        // Isso aqui vai desequipar a arma atual
        SwitchOffEquippedGun();

        // Isso aqui vai EQUIPAR a proxima arma
        EquipNextGun(weaponsOnHold[weaponIndex]);

        Gun_Attributes ga = gunEquipped.GetComponent<Gun_Attributes>();
        animPlayer.AttachShotgun(1, ga.isShotgun);
        AmmoType = ga.typeOfAmmo;
        
        OnSwapWeapon();
        if(OnCanSwapWeapon != null && isVisible) OnCanSwapWeapon();

        SetGunMultipliersByTypeOfGun(playerGunMultipliers, GeneralStatesBulletPercentages);
    }
    private void SwitchOffEquippedGun()
    {
        Gun_Attributes g_Attributes = gunEquipped.GetComponent<Gun_Attributes>();
        g_Attributes.OnReload -= ReloadDesiredGun;

        if (g_Attributes.flameThrowerVFX != null) g_Attributes.flameThrowerVFX.Stop();

        gunEquipped.SetActive(false);
    }
    private void EquipNextGun(GameObject nextGun)
    {
        // Define a nova arma equipada e ativa
        gunEquipped = nextGun;
        gunEquipped.SetActive(true);

        // Adiciona o evento de recarregamento da nova arma
        Gun_Attributes ga = gunEquipped.GetComponent<Gun_Attributes>();
        ga.OnReload += ReloadDesiredGun;
        equippedGunAttributes = ga;
        


    }
    #endregion

    #region CheckingObjectsInRange

    ICollectable currentCollectable = null;
    IUpgradeable currentUpgradeable = null;
    IBuyableInScene currentBuyable = null;
    private void CheckObjectsInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, CollectRange, CollectableLayer);

        if (hit != null)
        {
            HandleUpgradeable(hit);
            HandleCollectable(hit);
            HandleBuyable(hit);
        }
        else
        {
            HandleExit();
        }
    }

    private void HandleUpgradeable(Collider2D hit)
    {
        IUpgradeable upgradeable = hit.GetComponent<IUpgradeable>();

        if (upgradeable != null && upgradeable != currentUpgradeable)
        {
            OnUpgradeableEnter(upgradeable);
        }
    }

    private void HandleCollectable(Collider2D hit)
    {
        ICollectable collectable = hit.GetComponent<ICollectable>();

        if (collectable != null && collectable != currentCollectable)
        {
            OnCollectableEnter(collectable);
        }
        else if (collectable == null && currentCollectable != null)
        {
            OnCollectableExit();
        }
    }

    private void HandleBuyable(Collider2D hit)
    {
        IBuyableInScene buyable = hit.GetComponent<IBuyableInScene>();

        if (buyable != null && buyable != currentBuyable)
        {
            OnBuyableEnter(buyable);
        }
        else if (buyable == null && currentBuyable != null)
        {
            OnBuyableExit();
        }
    }

    private void HandleExit()
    {
        if (currentUpgradeable != null)
        {
            OnUpgradeableExit();
        }

        if (currentCollectable != null)
        {
            OnCollectableExit();
        }

        if (currentBuyable != null)
        {
            OnBuyableExit();
        }
    }

    private void OnUpgradeableEnter(IUpgradeable upgradeable)
    {
        Debug.Log("Upgradeable entrou no alcance!");
        currentUpgradeable = upgradeable;
        currentUpgradeable.ShowInputCondition(this);
    }

    private void OnUpgradeableExit()
    {
        Debug.Log("Upgradeable saiu do alcance!");
        currentUpgradeable.HideInputCondition();
        currentUpgradeable = null;
    }

    private void OnCollectableEnter(ICollectable collectable)
    {
        Debug.Log("Coletável entrou no alcance!");
        currentCollectable = collectable;
        gunWindowAttributes.SetActive(true);
        canCollect = true;
        Gun_Attributes gunPlayer = null;
        if(gunEquipped != null) gunPlayer = gunEquipped.GetComponent<Gun_Attributes>();
        currentCollectable.ShowInfos(gunWindowAttributes, gunPlayer);
    }

    private void OnCollectableExit()
    {
        Debug.Log("Coletável saiu do alcance!");
        canCollect = false;
        gunWindowAttributes.SetActive(false);
        currentCollectable = null;
    }

    private void OnBuyableEnter(IBuyableInScene buyable)
    {
        Debug.Log("Comprável entrou no alcance!");
        currentBuyable = buyable;
        currentBuyable.ShowStatsAndInfos();

        if (Input.GetKeyDown(KeyCode.F))
        {
            currentBuyable.Buy(metalCash);
        }
    }

    private void OnBuyableExit()
    {
        Debug.Log("Comprável saiu do alcance!");
        currentBuyable = null;
    }

    #endregion
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, CollectRange);
    }
    public void GetOrChangeWeaponSelected(GameObject gun, bool DoesAlreadyCollectedThisGunBefore)
    {
        facingRight = GetComponent<Player_Movement>().facingRight;

        if(gunEquipped != null)
        {
            gunEquipped.GetComponent<Gun_Attributes>().OnReload -= ReloadDesiredGun;
            gunEquipped.SetActive(false);
        }

        if (weaponsOnHold.Count < numeroMaximoDeArmasEmMao && weaponsOnHold.Count > 0)
        {
            weaponIndex++;
        }else if (weaponsOnHold.Count == numeroMaximoDeArmasEmMao)
        {
            
            
            weaponsOnHold.Remove(gunEquipped);
        }

        weaponsOnHold.Add(gun);
        
        Gun_Attributes ga = gun.GetComponent<Gun_Attributes>();
        Vector3 mirrored = new Vector3(gunPosition.localScale.x * -1, gunPosition.localScale.y, gunPosition.localScale.z);
        if (!facingRight) gunPosition.localScale = mirrored;

        if (DoesAlreadyCollectedThisGunBefore == false)
        {
            gun.transform.parent = gunPosition; gun.transform.position = gunPosition.position;

            if (ga.correctX != 0 || ga.correctY != 0) gun.transform.localPosition = new Vector3(ga.correctX, ga.correctY, gun.transform.localPosition.z);
            
        }

        gun.transform.rotation = gunPosition.rotation;

        gunEquipped = gun;
        equippedGunAttributes = ga;
        gunEquipped.SetActive(true);
        //gunEquipped.GetComponent<AlignParentToChild>().GetFixedPosition(gunPosition, facingRight);

        AmmoType = ga.typeOfAmmo; //gunEquipped.GetComponent<Gun_Attributes>().typeOfAmmo;
        
        ga.OnReload += ReloadDesiredGun;
        if (OnGetWeapon != null) OnGetWeapon();

        
        animPlayer.AttachShotgun(1, ga.isShotgun);
        SetGunMultipliersByTypeOfGun(playerGunMultipliers, GeneralStatesBulletPercentages);
        //animPlayer.AnimSwitchGun();

        if (!facingRight) gunPosition.localScale = new Vector3(gunPosition.localScale.x * -1, gunPosition.localScale.y, gunPosition.localScale.z);
    }

    void DropGun()
    {

    }
    #endregion

    public bool CheckDesiredAmmoAmmount(ammoTypeOfGunEquipped typeAmmo)
    {
        switch (typeAmmo)
        {
            case ammoTypeOfGunEquipped.AR:
                return ARammount > 0;

            case ammoTypeOfGunEquipped.Sub:
                return subAmmount > 0;
                
            case ammoTypeOfGunEquipped.Shotgun:
                return shotgunAmmount > 0;

            case ammoTypeOfGunEquipped.Pistol:
                return pistolAmmount > 0;

            case ammoTypeOfGunEquipped.FlameThrower:
                return flamethrowerAmmount > 0;
                
            case ammoTypeOfGunEquipped.Granade_Launcher:
                return granadeLauncherAmmount > 0;

            case ammoTypeOfGunEquipped.Rocket_Launcher:
                return rocketLauncherAmmount > 0;
                
        }
        return false;
    }

    public void AddCash(int valueToAdd)
    {
        metalCash += valueToAdd;
        Debug.Log("Ganhou");
        if (OnChangeCash != null) OnChangeCash(metalCash); //Evento de quando o player obtiver Cash...Se necessário, apenas criar msm
    }
    public void DecreaseCash(int valueToDescrease)
    {
        metalCash -= valueToDescrease;
        if (OnChangeCash != null) OnChangeCash(metalCash);
    }

    void BuyingTraps(int itemID)
    {
        _isBuilding = true;
        string path = $"test/{itemID}";
        GameObject obj = Resources.Load<GameObject>(path);
        objectToTransport = Instantiate(obj, dropPosition.position, Quaternion.identity);
        
    }

    void ShowBuidingSet()
    {
        //Debug.Log("Mostrou O bixo prestes a ser comprado");
    }

    public void StopAllPlayerMovement()
    {
        
        pMovement.StopPlayerMovement();
        animPlayer.StopOrContinueAnimations(false);
        
        pTracker.SwitchTrackAble(false);

        if (gunEquipped != null) { gunEquipped.GetComponent<Gun_Shoot>().SwitchAbleGunShoot(false); }
    }

    public void ContinueAllPlayerMovement()
    {
        
        pTracker.SwitchTrackAble(true);
        animPlayer.StopOrContinueAnimations(true);
        pMovement.ContinuePlayerMovement();

        if (gunEquipped != null) { gunEquipped.GetComponent<Gun_Shoot>().SwitchAbleGunShoot(true); }
    }

    public void OnSuccefullBuy(int price)
    {
        metalCash -= price;
    }

}

public interface ICollectable
{
    void Collect();
    void ShowInfos(GameObject uiWindowManager, Gun_Attributes attributesToCompare);
}

public interface IDamageable
{
    void TakeDamage(int damage, bool shouldPlayDamageAnim = true);
    void SetStun(bool hasToStun = true);

    /// <summary>
    /// Aplica um tipo de efeito determinado ao inimigo
    /// </summary>
    /// <param name="newState">Tipo de efeito que será aplicado</param>
    /// <param name="duration">Duração do efeito que será aplicado</param>
    /// <param name="DOTtime">Se houver, determina o intervalo a cada dano tomado dentro do efeito (Apenas Fire)(Recomendavel ser 1f)</param>
    void ApplyNaturalState(NaturalStates newState, float duration, float DOTtime = 1f);
}

public interface IBuyableInScene
{
    void Buy(int playerCash);
    void ShowStatsAndInfos();
}

public interface IUpgradeable
{
    void ShowInputCondition(PlayerInventory pInventory);
    void HideInputCondition();
}

public interface IThrowable
{
    void ThrowObject(Vector3 mousePosition, Vector3 launchPos);
    void SetDamage(int newDamage);
    void OnHitObject();
}

public interface IThrowableEffect
{
    void SetThrowableData(ThrowablesSO throwableData);
    void ApplyEffect(GameObject hitObject, int damage);
}