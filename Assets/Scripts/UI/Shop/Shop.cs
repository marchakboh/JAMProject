using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private ShopItem _itemTemplate, _emptyItemTemplate;
    [SerializeField]
    private GameObject _showcase;
    [SerializeField]
    private int _visibleItemsCount = 3;

    private int _itemsCount = 0;
    private int _currentIndex = 1;
    public bool isAlive = false;
    private void Awake()
    {
        isAlive = true;
        var _shopObjects = Resources.LoadAll<ShopObject>("Scriptable Objects/Shop").ToList();
        _itemsCount = _shopObjects.Count;
        AddEmptyCells(_visibleItemsCount / 2);
        foreach (var obj in _shopObjects)
        {
            var item = Instantiate(_itemTemplate);
            item.GetComponent<ShopItem>().Initialize(obj);
            item.transform.SetParent(_showcase.transform);
        }
        AddEmptyCells(_visibleItemsCount / 2);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void AddEmptyCells(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var item = Instantiate(_emptyItemTemplate);
            item.transform.SetParent(_showcase.transform);
        }
    }
    public bool SelectItemAt(int index)
    {
        if (index >= _visibleItemsCount / 2 && index < _itemsCount + _visibleItemsCount / 2)
        {
            if (index > _currentIndex)
            {
                for (int i = 0; i < index - _visibleItemsCount / 2; i++)
                    _showcase.transform.GetChild(i).GetComponent<ShopItem>().Active = false;
            }
            else
            {
                for (int i = index - _visibleItemsCount / 2; i < _currentIndex; i++)
                    _showcase.transform.GetChild(i).GetComponent<ShopItem>().Active = true;
            }
            _currentIndex = index;
            return true;
        }
        return false;
    }
    public void BuyCenterElement(){
        _showcase.transform.GetChild(_currentIndex).GetComponent<ShopItem>().BuyItem();
    }
}
