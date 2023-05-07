using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopUiControl : MonoBehaviour
{
    [SerializeField] private GameObject PlayerCharacter;

    private ControlsInput.ShopActionActions actions;
    private GameObject shopObject;
    private Shop shopInstance;
    private int startIndex = 1;

    private void Start()
    {
        shopObject = transform.Find("Shop").gameObject;
        shopInstance = shopObject.GetComponent<Shop>();
        shopInstance.SelectItemAt(startIndex);
    }

    public void OpenShop()
    {
        shopObject.SetActive(true);
    }

    public void SetActions(ControlsInput.ShopActionActions shopAction)
    {
        actions = shopAction;

        actions.MoveActionRight.performed += TryShopRight;
        actions.MoveActionLeft.performed += TryShopLeft;
        actions.BuyButtonPressed.performed += TryBuy;
        actions.Close.performed += CloseShop;
    }

    private void TryShopRight(InputAction.CallbackContext ctx)
    {
        Debug.Log("Left");
        startIndex++;
        if (!shopInstance.SelectItemAt(startIndex))
            startIndex--;
    }

    private void TryShopLeft(InputAction.CallbackContext ctx)
    {
        startIndex--;
        if (!shopInstance.SelectItemAt(startIndex))
            startIndex++;
    }

    private void TryBuy(InputAction.CallbackContext ctx)
    {
        if (shopInstance.BuyCenterElement())
        {
            PlayerCharacter.GetComponent<AlpacaCharacter>().UnlockEquipment(shopInstance.GetCurrentName(), shopInstance.GetCurrentPrice(), shopInstance.GetCurrentBonus());
        }
    }

    private void CloseShop(InputAction.CallbackContext ctx)
    {
        shopObject.SetActive(false);
        AlpacaCharacter character = PlayerCharacter.GetComponent<AlpacaCharacter>();
        if (character)
        {
            character.RestoreControls();
        }
    }
}
