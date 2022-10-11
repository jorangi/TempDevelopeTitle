using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum moment
    {
        Immediately = 0,                //��� �ߵ�
        TurnEnd,                        //�������� �� ����� �ߵ�
        TurnStart,                      //�������� �� ���۽� �ߵ�
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
        Damage = 0,                     //Damage        ������ : ���ظ� ������.
        Heal,                           //Damage        ȸ�� : ü���� ȸ���Ѵ�.
        Mana,                           //Independant   ����ȸ�� : ������ ȸ���Ѵ�.
        Inflict,                        //Independant       ���ϴ� ���ط� : ���ϴ� ���ط��� �����ϰų� �����Ѵ�.
        Receive,                        //Independant       �޴� ���ط� : �޴� ���ط��� �����ϰų� �����Ѵ�.
        Burn,                           //Burn          ȭ�� : ����� ������ ���ط��� 10% �����ϰ� �� ����� 3�� �������� ������.
        Poison,                         //Poison        �� : ����� �޴� ���ط��� 10% �����ϰ� �� ����� 2�� �������� ������.
    }
}
