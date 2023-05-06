using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item")]
public class ShopObject : ScriptableObject
{
    [SerializeField]
    private Sprite _image;
    public Sprite Image => _image;

    [SerializeField]
    private string _name;
    public string Name => _name;

    [SerializeField]
    private int _price;
    public int Price => _price;
}
