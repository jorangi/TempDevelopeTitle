using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum moment
    {
        Immediately = 0,                //즉시 발동
        TurnEnd,                        //시전자의 턴 종료시 발동
        TurnStart,                      //시전자의 턴 시작시 발동
    }
    public enum effType
    {
        Independant = 0,
        Damage,
        Heal,
        Disturbance,
        Burn,
        Poison
    }
    public enum eff
    {
        Damage = 0,                     //Damage        데미지 : 피해를 입힌다.
        Heal,                           //Damage        회복 : 체력을 회복한다.
        Mana,                           //Independant   마나회복 : 마나를 회복한다.
        Inflict,                        //Independant       가하는 피해량 : 가하는 피해량이 증가하거나 감소한다.
        Receive,                        //Independant       받는 피해량 : 받는 피해량이 증가하거나 감소한다.
        Burn,                           //Burn          화상 : 대상의 입히는 피해량이 10% 감소하고 턴 종료시 3의 데미지를 입힌다.
        Poison,                         //Poison        독 : 대상의 받는 피해량이 10% 증가하고 턴 종료시 2의 데미지를 입힌다.
    }
}
