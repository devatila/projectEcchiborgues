using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuySystem : MonoBehaviour
{
    public event Action<int> OnBuyItem;
    public event Action OnOpenShopMenu;
    public event Action OnCloseShopMenu;

    private PlayerInventory pInventory;
    [SerializeField] private GameObject shopMenu;
    public Button[] buttons;

    private bool CanShop = true;
    void Start()
    {
        pInventory = FindObjectOfType<PlayerInventory>();
        OnBuyItem += HideShopMenu;
        OnBuyItem += BlockShopWhileBuilding;

        OnOpenShopMenu += pInventory.StopAllPlayerMovement;
        OnCloseShopMenu += pInventory.ContinueAllPlayerMovement;

        pInventory.OnBuildPlaced += AllowShopAfterBuild;

        SetParentButtons();
        if (shopMenu.activeSelf == true)
        {
            shopMenu.SetActive(false);
        }
    }

    public void OnClickBuy(int itemPrice, UI_ItemValue itemValuePrice, Button botao)
    {
        //CheckIntemPrice(itemPrice, itemValuePrice, botao); // Parado por causar bugs na seleção de torretas
                                                             // CORRIGIR ISSO DEPOIS
        int price = itemValuePrice.itemPrice;
        int playerCash = pInventory.metalCash;
        
        
        if(playerCash >= price)
        {
            Debug.Log("O Player Tem Dinheiro Para Comprar");
            pInventory.DecreaseCash(price);
            OnBuyItem?.Invoke(itemValuePrice.itemID); //Trocar para ItemID dps
            pInventory.ContinueAllPlayerMovement();
        }
        else
        {
            Debug.LogWarning("O Player não tem dinheiro para comprar");
        }
    }

    void CheckIntemPrice(int valueA, UI_ItemValue valueB, Button btn)
    {
        
        if (valueA != valueB.itemPrice)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickBuy(valueB.itemPrice, valueB, btn));
        }
    }

    void HideShopMenu(int cumprirTabela)
    {
        shopMenu.SetActive(false);
    }

    void ShowShopMenu()
    {
        shopMenu.SetActive(true);
    }

    void SetParentButtons()
    {
        buttons = shopMenu.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            
            if (button.CompareTag("UIBuyerButton"))
            {
                GameObject parentObject = button.transform.parent.gameObject;

                UI_ItemValue ItemValue = parentObject.GetComponent<UI_ItemValue>();

                if (ItemValue != null)
                {
                    int itemPrice = ItemValue.itemPrice;

                    button.onClick.RemoveAllListeners();

                    button.onClick.AddListener(() => OnClickBuy(itemPrice, ItemValue, button));

                    //Debug.Log($"Botão: {button.name}, Pai: {parentObject.name}, ItemPrice: {itemPrice}");
                }
                else
                {
                    Debug.LogWarning($"UI_ItemValue não encontrado no pai do botão: {button.name}");
                }

            }
        }
    }

    
    public void BlockShopWhileBuilding(int t)
    {
        CanShop = false;
    }

    public void AllowShopAfterBuild()
    {
        CanShop = true;
    }

    public bool IsShopOpen()
    {
        return shopMenu.activeSelf;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && CanShop == true)
        {
            if (shopMenu.activeSelf)
            {
                shopMenu.SetActive(false);
                OnCloseShopMenu?.Invoke();
            }
            else
            {
                shopMenu.SetActive(true);
                OnOpenShopMenu?.Invoke();
            }
        }
    }
}
