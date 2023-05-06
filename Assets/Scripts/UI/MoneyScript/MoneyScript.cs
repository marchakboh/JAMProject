using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New Item", menuName = "Money/UI")]
public class InfoObject : ScriptableObject
{
    [SerializeField]
    private string money;
    public string Money => money;


    [SerializeField]
    private string respect;
    public string Respect => respect;

}
public class MoneyScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _money;

    [SerializeField]
    private TextMeshProUGUI _respect;

    static public int Money;
    static public int Respect;
    public void AddMoney(int plusMoney)
    {
        Money = Money+plusMoney;
    }
    public void SubstractMoney(int money)
    {
        Money = Money-money;
    }
    public void addRespect(int respect)
    {
        Respect = Respect + respect;
    } 
    public int getMoneyValue(){
        return Money;
    }
    void Start()
    {
        Money = 100;
        Respect = 0;
        _money.text = Money.ToString();
        _respect.text = Respect.ToString();
    }

    // Update called once per frame
    void UpdateInfo()
    {
        _money.text = Money.ToString();
        _respect.text = Respect.ToString();
    }
}
