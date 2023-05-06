using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTest : MonoBehaviour
{
    [SerializeField]
    private Shop _shop;

    private int startIndex = 1;

    private void Start()
    {
        _shop.SelectItemAt(startIndex);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            startIndex++;
            if (!_shop.SelectItemAt(startIndex))
                startIndex--; ;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            startIndex--;
            if (!_shop.SelectItemAt(startIndex))
                startIndex++;
        }
        //ToDo to player controller
      /*  else if(Input.GetKeyDown(KeyCode.E)){
            if (!_shop.isAlive)
            {
                _shop.Open();
            }
            else _shop.Close();
        }*/
    }
}
