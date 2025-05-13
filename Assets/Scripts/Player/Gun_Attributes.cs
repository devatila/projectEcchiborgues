using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun_Attributes : MonoBehaviour
{
    public delegate void ToReload(int maxMagazine);
    public event ToReload OnReload;

    public PlayerInventory.ammoTypeOfGunEquipped typeOfAmmo;
    public WeaponDataSO weaponDataSO;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public Sprite gunSprite;
    [HideInInspector] public string gunName = "Nome da Arma Aqui Pls"; //Por favor não esquecer pelo nosso bem
    [HideInInspector] public int gunId = 1; //0 = Quando o player está sem arma; gunId > 0 = quando o player tem alguma arma na mão
    // ENTRE 1 E 2 POIS ISSO DEFINIRA SE A ARMA É DE UMA MÃO OU DUAS MÃOS

    [HideInInspector] public int gunDamage;
    [HideInInspector] public float cadency;


    [HideInInspector] public float minSpread;
    [HideInInspector] public float maxSpread;
    [HideInInspector] public float decreaseSpreadSpeed, SpreadForce;
    [HideInInspector] public float actualSpread;


    [HideInInspector] public bool reloadable;
    [HideInInspector] public int magazine = 10;

    public int actual_magazine;
    [HideInInspector] public float reloadTime = 3f;
    [HideInInspector] public bool isReloading;

    [HideInInspector] public bool isShotgun;
    [HideInInspector] public int shotgunFragmentsQuantitative = 10;
    [HideInInspector] public float MaxDistance;
    [HideInInspector] public float damageReductionRate = 0.5f;
    //Eu sei que fica feio assim, mas estava pior antes...

    public bool hasSpecialProjectile { get; private set;}
    public GameObject customProjectile { get; private set;}

    public bool instanciedProjectile { get; private set;}
    public int flTickCost { get; private set;}

    public GameObject projectile;
    public Transform shootPosition;
    public float correctX; //Gambiarras...eu sei, me perdoe
    public float correctY;

    public Transform capsulePos;

    private AnimPlayer animPlayer;
    private Gun_Shoot g_Shoot;
    public PlayerInventory playerInventory { get; set; }
    private int maxMagazine;

    public int customID { get; private set; }
    public ParticleSystem flameThrowerVFX;
    private GameObject ftTransform;
    private RechargeEffect m_RechargeEffect;

    private void Awake()
    {
        GetSOdata(weaponDataSO);
    }
    private void Start()
    {
        if(flameThrowerVFX != null)
        {
            ftTransform = new GameObject("FT Transform");
            ftTransform.transform.parent = flameThrowerVFX.transform.parent;
            ftTransform.transform.position = flameThrowerVFX.transform.position;
            ftTransform.transform.rotation = flameThrowerVFX.transform.rotation;

            flameThrowerVFX.transform.parent = null;
            flameThrowerVFX.transform.localScale = Vector3.one;
        }

        var c = GetComponent<CustomProjectileForGunManager>();
        if (c != null)
        {
            m_RechargeEffect = GetComponentInParent<RechargeEffect>();
        }

        maxMagazine = magazine;
        if(customProjectile != null) projectile = customProjectile;
        if (hasSpecialProjectile && projectile != null)
        {
            customID = projectile.GetComponent<CustomProjectileScript>().customID;
        }
        animPlayer = FindObjectOfType<AnimPlayer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        g_Shoot = GetComponent<Gun_Shoot>();
        if(spriteRenderer != null) spriteRenderer.sprite = gunSprite;

        actualSpread = minSpread;
        playerInventory = transform.parent.parent.parent.parent.parent.parent.parent.GetComponentInParent<PlayerInventory>();
        //projectile.GetComponent<Projectile>().damage = gunDamage;
        playerInventory.OnStopRelaod += stopReloadGun;
    }

    public void Reload()
    {
        isReloading = true;
        g_Shoot.isReloading = true;
        animPlayer.ReloadGun(reloadTime);
        StartCoroutine(Reloader(reloadTime));
    }

    IEnumerator Reloader(float time)
    {
        Debug.Log("Chamou Reload");
        yield return new WaitForSeconds(time);
        if(OnReload != null)
        {
            int result = maxMagazine - actual_magazine;
            OnReload(result);

        }
        isReloading = false;
        g_Shoot.isReloading = false;
        
        //actual_magazine = magazine;
    }

    public void stopReloadGun()
    {
        StopAllCoroutines();
        CheckCustomProjAndHidingIt();
        Debug.Log("Parando de Recarregar");
    }

    void CheckCustomProjAndHidingIt() // Eu e meus nomes de milhões....
    {
        var c = GetComponent<CustomProjectileForGunManager>();
        if(c != null)
        {
            if(m_RechargeEffect.lastObject != null) m_RechargeEffect.lastObject.SetActive(false);
            else
            {
                StopAllCoroutines();
                if (OnReload != null)
                {
                    int result = maxMagazine - actual_magazine;
                    OnReload(result);

                }
                isReloading = false;
                g_Shoot.isReloading = false;
            }
        }
    }
    private void OnEnable()
    {
        
        if (isReloading)
        {
            if (actual_magazine == 0)
            {
                Reload();
                Debug.Log("Tentando Recarregar De novo");
            } else
            {
                isReloading = false;
                g_Shoot.isReloading = false;
            }
        }
    }
    void GetSOdata(WeaponDataSO WeaponSO)
    {
        if (weaponDataSO == null) return;
        gunSprite                             = WeaponSO.gunSprite;
        gunName                               = WeaponSO.gunName;
        gunId                                 = WeaponSO.gunId;
        gunDamage                             = WeaponSO.gunDamage;
        cadency                               = WeaponSO.cadency;
        minSpread                             = WeaponSO.minSpread;
        maxSpread                             = WeaponSO.maxSpread;
        decreaseSpreadSpeed                   = WeaponSO.decreaseSpreadSpeed;
        SpreadForce                           = WeaponSO.SpreadForce;
        reloadable                            = WeaponSO.reloadable;
        magazine                              = WeaponSO.magazine;
        actual_magazine                       = WeaponSO.magazine;
        reloadTime                            = WeaponSO.reloadTime;
        isShotgun                             = WeaponSO.isShotgun;
        shotgunFragmentsQuantitative          = WeaponSO.shotgunFragmentsQuantitative;
        MaxDistance                           = WeaponSO.MaxDistance;
        damageReductionRate                   = WeaponSO.damageReductionRate;
        hasSpecialProjectile                  = WeaponSO.isSpecialProjectile;
        customProjectile                      = WeaponSO.customProjectile;
        typeOfAmmo                            = WeaponSO.ammoType;
        instanciedProjectile                  = WeaponSO.instantiatedProjectile;
        flTickCost                            = WeaponSO.AmmoCostPerTick;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            bool hasAmmo = playerInventory.CheckDesiredAmmoAmmount(typeOfAmmo);
            Debug.Log(hasAmmo + " " + (actual_magazine == maxMagazine));
            if (!hasAmmo || actual_magazine == maxMagazine) return;
            Reload();
        }

        if (flameThrowerVFX != null)
        {
            flameThrowerVFX.transform.position = ftTransform.transform.position;
            flameThrowerVFX.transform.rotation = ftTransform.transform.rotation;
            
        }
    }
}
