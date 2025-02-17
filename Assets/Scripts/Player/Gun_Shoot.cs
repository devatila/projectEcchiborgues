using System;
using System.Collections;
using UnityEngine;

public class Gun_Shoot : MonoBehaviour
{
    // Delegates e Eventos
    public delegate void ShootAdvise(int value);
    public ShootAdvise OnShootUpdate;

    // Atributos da arma
    public Gun_Attributes _gunAttributes;
    public GameObject trajectorySpritePrefab;

    // Estado da Arma
    [HideInInspector] public bool can_Shoot = true;
    private bool isShooting = false;

    // Referências
    public GameObject MouseAcompanhador, projectileTest;
    public SpriteRenderer circleSprite;
    private Pool_Projectiles projectilesPool;
    private bool stopMouseMoveReading;

    private Transform shootPosition;
    private AnimPlayer animPlayer;
    private Player_Effects player_Effects;

    private bool isCadencyRunning = false;
    public bool isReloading { get; set; }
    private Coroutine cadencyCoroutine;
    private bool mouseOnRightPosition;
    private Coroutine flameThrowerCoroutine;

    private void OnEnable()
    {
        ResetState();
        animPlayer = FindObjectOfType<AnimPlayer>();
    }

    public void SwitchAbleGunShoot(bool state)
    {
        can_Shoot = state;
        if (isCadencyRunning)
        {
            
            if (cadencyCoroutine != null)
            {
                StopCoroutine(cadencyCoroutine);
                cadencyCoroutine = null;
                can_Shoot = false;
            }
        }
    }
    private void ResetState()
    {
        can_Shoot = true;
        isShooting = false;
    }

    void Start()
    {
        InitializeReferences();
        SetupMouseSprite();
        SetupShootPosition();
        SubscribeToEvents();

        
    }

    private void InitializeReferences()
    {
        player_Effects = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Effects>();
        projectilesPool = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<Pool_Projectiles>();
        _gunAttributes = GetComponent<Gun_Attributes>();
        MouseAcompanhador = GameObject.FindGameObjectWithTag("MouseAcompanhador");
        circleSprite = MouseAcompanhador.GetComponentInChildren<SpriteRenderer>();
        stopMouseMoveReading = true;
    }

    private void SetupMouseSprite()
    {
        circleSprite.transform.localScale = new Vector3(_gunAttributes.minSpread * 2f, _gunAttributes.minSpread * 2f, 1f);
    }

    private void SetupShootPosition()
    {
        shootPosition = _gunAttributes.shootPosition ?? transform;
    }

    private void SubscribeToEvents()
    {
        TypewriterEffectTMP.stopAll += StopMouseRead;
        TypewriterEffectTMP.ContinueAll += ContinueMouseRead;
    }

    void Update()
    {
        if (stopMouseMoveReading)
        {
            GetShootData(_gunAttributes.gunDamage, _gunAttributes.cadency);
        }

        SpreadManager();
    }

    private void OnDrawGizmos()
    {
        if (MouseAcompanhador != null)
            Gizmos.DrawWireSphere(MouseAcompanhador.transform.position, _gunAttributes.actualSpread);
    }

    private void GetShootData(int damage, float cadency)
    {
        Vector3 mousePosition = GetMousePosition();

        mouseOnRightPosition = mousePosition.x > transform.position.x;

        MouseAcompanhador.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        HandleShootingInput();

        if (isShooting && can_Shoot && _gunAttributes.actual_magazine >= 0)
        {
            if (_gunAttributes.typeOfAmmo == PlayerInventory.ammoTypeOfGunEquipped.FlameThrower) return;
            Shoot();
            
            cadencyCoroutine = StartCoroutine(CadencyShoot(damage, cadency));
            
        }
    }

    private Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    bool CheckHasFT()
    {
        return _gunAttributes.typeOfAmmo == PlayerInventory.ammoTypeOfGunEquipped.FlameThrower;
    }
    private void HandleShootingInput()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            if (CheckHasFT()) FlameThrowerShoot();
            isShooting = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (CheckHasFT()) StopFlameThrowerShoot(); //Percebi
            isShooting = false;
        }
    }

    private void Shoot()
    {
        if (isReloading) return;

        if (_gunAttributes.actual_magazine > 0)
        {
            if (!_gunAttributes.isShotgun)
            {    
                ShootSingleProjectile();
            }
            else
            {
                ShootShotgunProjectiles();
            }

            UpdateMagazine();
            UpdateSpread();
            CameraShake.instance.Shake();


        }
    }

    private void FlameThrowerShoot()
    {
        //Debug.Log("Lançando Chamas");
        
        _gunAttributes.flameThrowerVFX.gameObject.GetComponent<PS_EmitterTest>().OnShoot();
        if (flameThrowerCoroutine == null)
            flameThrowerCoroutine = StartCoroutine(FlamethrowerMagazineLoop(_gunAttributes.cadency));

    }
    private void StopFlameThrowerShoot()
    {
        //Debug.Log("Parou De Lançar");
        _gunAttributes.flameThrowerVFX.gameObject.GetComponent<PS_EmitterTest>().OnUnshoot();
        if(flameThrowerCoroutine != null)
        {
            StopCoroutine(flameThrowerCoroutine);
            flameThrowerCoroutine = null; //Limpa a referencia
        }
    }

    IEnumerator FlamethrowerMagazineLoop(float tickCadency)
    {
        while (true)
        {
            if(isReloading) StopFlameThrowerShoot();

            _gunAttributes.actual_magazine -= _gunAttributes.flTickCost;
            if (!HasFlameAmmo())
            {
                _gunAttributes.actual_magazine = 0;
                StopFlameThrowerShoot();
            }
            OnShootUpdate?.Invoke(_gunAttributes.actual_magazine);
            CheckNeedReloadAFterShoot();
            yield return new WaitForSeconds(tickCadency);
        }
    }
    bool HasFlameAmmo() => _gunAttributes.actual_magazine > 0;
    private void ShootSingleProjectile()
    {
        Vector3 randomPosition = GetRandomPositionInCircle(_gunAttributes.actualSpread);
        switch (_gunAttributes.hasSpecialProjectile)
        {
            case true:
                SetupCustomProjectile(randomPosition);
                break;

            case false:
                
                GameObject projectile = projectilesPool.GetObject();
                SetupProjectile(projectile, randomPosition);

                CreateTrajectory(projectile, randomPosition);
                break;
        }

        PlayShootEffects(false);
    }

    private void SetupCustomProjectile(Vector3 RandomPosition)
    {
        if (_gunAttributes.instanciedProjectile)
        {
            GameObject p = _gunAttributes.projectile;
            p.GetComponent<CustomProjectileScript>().isActive = true;
            p.transform.parent = null;

            Vector3 direction = p.transform.position - RandomPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            p.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
            p.GetComponent<CustomProjectileScript>().localDamage = _gunAttributes.gunDamage;

            if (!mouseOnRightPosition)
            {
                p.transform.localScale = new Vector3(p.transform.localScale.x * -1, p.transform.localScale.y, p.transform.localScale.z);
            }

        }
        else
        {
            // Método em que o comportamento do projetil fica por conta do seu proprio script
            GameObject projectile = Instantiate(_gunAttributes.customProjectile); //Dps trocar para pooling

            projectile.transform.position = shootPosition.position;

            Vector3 direction = shootPosition.position - RandomPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Ajusta o dano no script do projetil
            projectile.GetComponent<CustomProjectileScript>().localDamage = _gunAttributes.gunDamage;
        }
    }

    private void SetupProjectile(GameObject projectile, Vector3 position)
    {
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;
        projectile.GetComponent<Projectile>().damage = _gunAttributes.gunDamage;
    }

    private void CreateTrajectory(GameObject projectile, Vector3 position)
    {
        GameObject trajectory = projectilesPool.GetFlash();
        trajectory.transform.position = (shootPosition.position + position) / 2;
        AdjustTrajectory(trajectory, projectile.transform.position);
    }

    private void AdjustTrajectory(GameObject trajectory, Vector3 targetPosition)
    {
        Vector3 direction = trajectory.transform.position - targetPosition; //Eu acho que esqueci de Normalizar isso aqui...
        float distance = direction.magnitude;

        trajectory.transform.right = direction.normalized;
        trajectory.transform.localScale = new Vector3(distance * 0.1f * -1, 0.09f, 1f);
    }

    private void PlayShootEffects(bool isRechargeable)
    {
        animPlayer.PlayShootAnimation();
        player_Effects.showMuzzle();
        if(!isRechargeable) ShowCapsules();
    }

    private void ShootShotgunProjectiles()
    {
        for (int i = 1; i <= _gunAttributes.shotgunFragmentsQuantitative; i++)
        {
            Vector3 randomPosition = GetRandomPositionInCircle(_gunAttributes.actualSpread);
            GameObject projectile = projectilesPool.GetObject();
            
            SetupProjectile(projectile, randomPosition);

            CreateTrajectory(projectile, randomPosition);

            AdjustShotgunDamage(projectile, randomPosition);
        }
        PlayShootEffects(true);
    }

    private void AdjustShotgunDamage(GameObject projectile, Vector3 randomPosition)
    {
        float distance = Vector3.Distance(transform.position, randomPosition);
        int damage = _gunAttributes.gunDamage / _gunAttributes.shotgunFragmentsQuantitative;

        if (distance > _gunAttributes.MaxDistance)
        {
            float extraDistance = distance - _gunAttributes.MaxDistance;
            damage = (int)Mathf.Max(damage * 0.5f, damage - (int)(extraDistance * _gunAttributes.damageReductionRate));
        }

        projectile.GetComponent<Projectile>().damage = damage;
    }

    private void UpdateMagazine()
    {
        if (CheckHasFT()) return;
        _gunAttributes.actual_magazine--;
        OnShootUpdate?.Invoke(_gunAttributes.actual_magazine);
        // CheckNeedReloadAFterShoot();
    }

    private void CheckNeedReloadAFterShoot()
    {
        //Debug.Log((_gunAttributes != null) + " " + (_gunAttributes.playerInventory != null));
        bool hasAmmo = _gunAttributes.playerInventory.CheckDesiredAmmoAmmount(_gunAttributes.typeOfAmmo);
        if (_gunAttributes.actual_magazine <= 0 && !isReloading && hasAmmo)
        {
            _gunAttributes.Reload();
            if(flameThrowerCoroutine != null)
            {
                StopCoroutine(flameThrowerCoroutine);
                
                flameThrowerCoroutine = null;
            }
            //animPlayer.ReloadGun(_gunAttributes.reloadTime);
        }
    }

    public void ShowCapsules()
    {
        if (_gunAttributes.capsulePos == null) return;

        GameObject capsule = projectilesPool.GetCapsules();
        capsule.transform.position = _gunAttributes.capsulePos.position;
        capsule.GetComponent<CapsuleLogic>().ToEnable(player_Effects.footPos.position.y);
    }

    private void SpreadManager()
    {
        if (_gunAttributes.actualSpread > _gunAttributes.minSpread)
        {
            _gunAttributes.actualSpread -= _gunAttributes.decreaseSpreadSpeed * Time.deltaTime;
            circleSprite.transform.localScale = new Vector3(_gunAttributes.actualSpread * 2f, _gunAttributes.actualSpread * 2f, 1f);
        }
    }

    private Vector3 GetRandomPositionInCircle(float radius)
    {
        Vector2 randomPos = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector3(randomPos.x, randomPos.y, 0) + MouseAcompanhador.transform.position;
    }

    private void UpdateSpread()
    {
        if (_gunAttributes.actualSpread < _gunAttributes.maxSpread)
        {
            float difference = _gunAttributes.maxSpread - _gunAttributes.actualSpread;
            _gunAttributes.actualSpread += Mathf.Min(difference, _gunAttributes.SpreadForce);
        }
    }

    IEnumerator CadencyShoot(int damage, float timer)
    {
        isCadencyRunning = true;
        can_Shoot = false;
        yield return new WaitForSeconds(timer);
        can_Shoot = true;

        CheckNeedReloadAFterShoot();

        isCadencyRunning = false;
        cadencyCoroutine = null;
    }

    #region Leitura_do_Mouse
    void StopMouseRead() => stopMouseMoveReading = false;
    void ContinueMouseRead() => stopMouseMoveReading = true;
    #endregion
}
