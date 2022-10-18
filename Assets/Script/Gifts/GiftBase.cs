using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBase : MonoBehaviour
{
    public virtual void Acquire()
    {
        //습득 시점
    }
    public virtual void Reroll()
    {
        //카드 재구성 시점
    }
    public virtual void Shuffle()
    {
        //카드 섞은 시점
    }
    public virtual void CardUse()
    {
        //카드 사용 시점
    }
    public virtual void Excute()
    {
        //카드 능력 발동 시점
    }
    public virtual void Hit()
    {
        //피격 시점
    }
    public virtual void BattleStart()
    {
        //전투 시작 시점
    }
    public virtual void Turnstart()
    {
        //턴 시작 시점
    }
    public virtual void TurnEnd()
    {
        //턴 종료 시점
    }
}
