using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressStruct : MonoBehaviour
{
    public int Money = 100;
    public int Respect = 0;
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
}
