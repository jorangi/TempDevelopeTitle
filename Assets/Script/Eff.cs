using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffType
{
    public Enums.effType effType;
    public float per;
    public int add;

    public EffType(float per, int add, Enums.effType effType = Enums.effType.Independant)
    {
        this.effType = effType;
        this.per = per;
        this.add = add;
    }
}
[System.Serializable]
public class Eff
{
    public Enums.effType effType;
    public Enums.eff eff;
    public Enums.moment moment;
    public int val, accum, turn;
    public bool run = false;
    public Character Caster;

    public Eff(Enums.effType effType, Enums.eff eff, int val, int accum, int turn, Character Caster, Enums.moment moment = Enums.moment.Immediately)
    {
        this.Caster = Caster;
        this.effType = effType;
        this.eff = eff;
        this.moment = moment;
        this.val = val;
        this.accum = accum;
        this.turn = turn;
    }
}