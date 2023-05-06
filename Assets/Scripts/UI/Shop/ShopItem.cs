using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private Image _mainImage;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Button _buyButton;
    [SerializeField]
    private TextMeshProUGUI _price;

    private int price = 0;
    private bool _active;
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            gameObject.SetActive(value);
        }
    }
    public void Initialize(ShopObject dataObject, bool isBought = false)
    {
        _mainImage.GetComponent<Image>().sprite = dataObject.Image;
        _name.text = dataObject.Name;
        price = dataObject.Price;
        if (isBought)
        {
            _buyButton.GetComponent<Button>().enabled = false;
            _price.text = "Sold";
        }
        else
        {
            _price.text = price.ToString();
        }
    }


    public void BuyItem()
    {
        ///TO DO Check moneys count of player and withdraw money from the balance
        _buyButton.GetComponent<Button>().enabled = false;
        _price.text = "Sold";
    }
}
