using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunWindowInfo : MonoBehaviour
{

    [Header("Informações da Arma Dropada")]
    public Sprite gunSprite;  // O sprite da arma que vai ser dado na função UpdateInfo()
    public int damageInfo;    // O dano da arma que será passado
    public float cadencyInfo; // A merma coisa de cima so que com a cadencia

    [Header("Atributos do HUD")]
    public Image iconSprite;
    public TMP_Text damageValue;
    public TMP_Text cadencyValue;
    public Image[] IconsReference;

    [Header("Controle de Posição")]
    public RectTransform hudWindow;                         // A janela de atributos no Canvas
    public float offset = 50f;                             // Offset da janela em relação à arma (em pixels)
    public Vector2 screenPadding = new Vector2(10f, 10f); // Padding da tela para não cortar o HUD
    public Transform player;                             // O transform do player
    public Transform weaponDrop;                        // A posição do item Dropado
    public float transitionSpeed = 1f;

    [Header("Variaveis de Controle de Comparação")]
    public Image checkDamage;
    public Image checkCadency;
    public Image checkFull;

    [Header("Varivaeis de Controle de Leitura de Munição")]
    public TMP_Text ammoAmmountText;


    private bool hasMoreDamage;   // Fica verdadeiro se a arma do chão tiver mais dano do que a arma na mão atual
    private bool hasMoreCadency;  // Fica verdadeiro se a arma do chão tiver mais cadencia do que a arma na mão atual
    private bool isFullOfGuns;    // Fica verdadeiro se o player ja estiver no limite de armas em mãos

    private Camera mainCamera;
    private PlayerInventory playerInventory;
    private Gun_Attributes temporaryGunAttributes;

    private void OnEnable()
    {
        //iconSprite.sprite = gunSprite;
        //iconSprite.SetNativeSize();

        //damageValue.text = damageInfo.ToString();
        //cadencyValue.text = cadencyInfo.ToString();


        
    }

    private void OnDisable()
    {
        gunSprite = null;
        damageInfo = 0;
        cadencyInfo = 0;

        iconSprite.sprite = null;
        damageValue.text = "";
        cadencyValue.text = "";
    }

    private void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player").transform;
        
        playerInventory = player.GetComponent<PlayerInventory>();
        playerInventory.OnCanSwapWeapon += SwapWeapon;
    }

    public void UpdateInfo(Transform weaponPosition, Gun_Attributes HandGunAttributes, Gun_Attributes dropedGunAttributes)
    {
        UnableOrAbleGunInfos(true);

        iconSprite.sprite = gunSprite;
        iconSprite.SetNativeSize();

        damageValue.text = damageInfo.ToString();
        cadencyValue.text = cadencyInfo.ToString();
        weaponDrop = weaponPosition;
        temporaryGunAttributes = dropedGunAttributes;

        if (player == null) player = FindObjectOfType<PlayerInventory>().gameObject.transform;

        if (HandGunAttributes != null)
        {
            hasMoreDamage = dropedGunAttributes.gunDamage > HandGunAttributes.gunDamage;
            hasMoreCadency = HandGunAttributes.cadency > dropedGunAttributes.cadency;
            isFullOfGuns = player.gameObject.GetComponent<PlayerInventory>().weaponsOnHold.Count == player.gameObject.GetComponent<PlayerInventory>().numeroMaximoDeArmasEmMao;
        }
        else
        {
            hasMoreDamage = dropedGunAttributes.gunDamage > 0;
            hasMoreCadency = dropedGunAttributes.cadency > 0;
            isFullOfGuns = false;
        }
        CompareIcons();

    }
    void UnableOrAbleGunInfos(bool status)
    {
        damageValue.enabled = status;
        cadencyValue.enabled = status;

        checkCadency.enabled = status;
        checkDamage.enabled = status;
        checkFull.enabled = status;

        foreach(Image i in IconsReference)
        {
            i.enabled = status;
        }

        //-------------------------------// 

        ammoAmmountText.enabled = !status;
    }
    void UnableOrAbleAmmoInfos(bool status)
    {
        
    }

    public void UpdateAmmoInfoGUI(Transform dropPosition, int ammoAmount, Sprite ammoSprite)
    {
        UnableOrAbleGunInfos(false);
        iconSprite.sprite = ammoSprite;
        iconSprite.SetNativeSize();

        weaponDrop = dropPosition;

        ammoAmmountText.text = "+" + ammoAmount.ToString();

    }

    public void UpdateCashInfo(Transform dropPosition, int cashAmmount, Sprite cashSprite)
    {
        UnableOrAbleAmmoInfos(false);
        iconSprite.sprite = cashSprite;
        iconSprite.SetNativeSize();

        weaponDrop = dropPosition;
        ammoAmmountText.text = "+" + cashAmmount.ToString();
    }

    void CompareIcons()
    {
        checkFull.enabled = false;
        if (hasMoreDamage) { checkDamage.color = Color.green; checkDamage.rectTransform.rotation = Quaternion.Euler(0, 0, 180); }
        else
        {
            checkDamage.color = Color.red;
            checkDamage.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (hasMoreCadency) { checkCadency.color = Color.green; checkCadency.rectTransform.rotation = Quaternion.Euler(0, 0, 180); }
        else
        {
            checkCadency.color = Color.red;
            checkCadency.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (isFullOfGuns) checkFull.enabled = true;
    }

    void SwapWeapon()
    {
        Gun_Attributes PI = player.gameObject.GetComponent<PlayerInventory>().gunEquipped.GetComponent<Gun_Attributes>();
        if(temporaryGunAttributes != null) UpdateInfo(weaponDrop, PI, temporaryGunAttributes);
    }

    private void Update()
    {
        UpdateHudPosition(false);
    }

    private void UpdateHudPosition(bool pinned)
    {
        if (weaponDrop == null) return;
        // Posição da arma no mundo para a tela
        Vector3 weaponScreenPos = mainCamera.WorldToScreenPoint(weaponDrop.position);

        // Verifica se o jogador está à direita ou à esquerda da arma
        bool isPlayerOnLeft = player.position.x < weaponDrop.position.x;

        // Ajusta o offset com base na resolução atual em comparação à resolução base (1920x1080)
        float scaleFactorX = Screen.width / 1920f;
        float scaleFactorY = Screen.height / 1080f;

        float scaledOffsetX = offset * scaleFactorX;
        float scaledOffsetY = offset * scaleFactorY;

        // Define a posição da janela com base na posição do jogador
        Vector3 hudPos = weaponScreenPos;
        if (isPlayerOnLeft)
        {
            // Coloca a janela à direita da arma
            hudPos.x += scaledOffsetX;
        }
        else
        {
            // Coloca a janela à esquerda da arma
            hudPos.x -= scaledOffsetX;
        }

        // Ajuste vertical para que a janela fique sempre acima do chão (por exemplo)
        hudPos.y += scaledOffsetY / 2;

        // Verifica os limites da tela
        hudPos = KeepHudInsideScreen(hudPos);

        Vector3 currentHudPos = hudWindow.position;

        // Atualiza a posição da HUD
        if (!pinned)
            hudWindow.position = Vector3.Lerp(currentHudPos, hudPos, Time.deltaTime * transitionSpeed);
        else
            hudWindow.position = hudPos;
    }

    private Vector3 KeepHudInsideScreen(Vector3 hudPos)
    {
        // Limites da tela com padding
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Impede que a HUD vá além dos limites da tela
        hudPos.x = Mathf.Clamp(hudPos.x, screenPadding.x, screenWidth - screenPadding.x);
        hudPos.y = Mathf.Clamp(hudPos.y, screenPadding.y, screenHeight - screenPadding.y);

        return hudPos;
    }

}
