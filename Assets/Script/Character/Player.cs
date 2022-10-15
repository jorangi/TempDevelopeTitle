using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public int gold = 0;
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            GameManager.Inst.battle.GoldText.text = $"{value} G";
        }
    }
    protected override void Start()
    {
        base.Start();
        mhp = 100;
        HP = mhp;
        Gold = 100;
        foreach (var card in GameManager.Inst.cardJson)
        {
            cards.Add(card["id"].ToString());
        }

        GameManager.Inst.battle.SetRandomCard();
        GameManager.Inst.battle.SetRandomShopCard();
    }
}
