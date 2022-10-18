using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBase : MonoBehaviour
{
    public virtual void Acquire()
    {
        //���� ����
    }
    public virtual void Reroll()
    {
        //ī�� �籸�� ����
    }
    public virtual void Shuffle()
    {
        //ī�� ���� ����
    }
    public virtual void CardUse()
    {
        //ī�� ��� ����
    }
    public virtual void Excute()
    {
        //ī�� �ɷ� �ߵ� ����
    }
    public virtual void Hit()
    {
        //�ǰ� ����
    }
    public virtual void BattleStart()
    {
        //���� ���� ����
    }
    public virtual void Turnstart()
    {
        //�� ���� ����
    }
    public virtual void TurnEnd()
    {
        //�� ���� ����
    }
}
