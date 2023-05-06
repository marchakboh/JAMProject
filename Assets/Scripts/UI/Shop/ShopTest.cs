using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTest : MonoBehaviour
{
    [SerializeField]
    private Shop _shop;

    private ControlsInput input;
    private bool isRightPress = false;
    private bool isLeftPress = false;
    private bool isBuybuttonPress = false;
    private int startIndex = 1;

    private void Awake()
    {
        input = new ControlsInput();
    }

    private void Start()
    {
        _shop.SelectItemAt(startIndex);

        input.ShopAction.MoveActionRight.performed += ctx => isRightPress = true;
        input.ShopAction.MoveActionRight.canceled  += ctx => isRightPress = false;

        input.ShopAction.MoveActionLeft.performed += ctx => isLeftPress = true;
        input.ShopAction.MoveActionLeft.canceled  += ctx => isLeftPress = false;
        
        input.ShopAction.BuyButtonPressed.performed += ctx => isBuybuttonPress = true;
        input.ShopAction.BuyButtonPressed.canceled  += ctx => isBuybuttonPress = false;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        if (isRightPress)
        {
            startIndex++;
            if (!_shop.SelectItemAt(startIndex))
                startIndex--;

            isRightPress = false;
        }
        else if (isLeftPress)
        {
            startIndex--;
            if (!_shop.SelectItemAt(startIndex))
                startIndex++;
            
            isLeftPress = false;
        }
        else if(isBuybuttonPress)
        {
            _shop.BuyCenterElement();
            isBuybuttonPress = false;
        }
    }
}
