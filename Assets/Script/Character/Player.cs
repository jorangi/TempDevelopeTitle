using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public int mhp;
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, mhp);

            GameManager.Inst.battle.HPText.text = $"{hp} / {mhp}";
            GameManager.Inst.battle.HPText.transform.parent.Find("HPBar").GetComponent<Image>().fillAmount = (float)hp / mhp;

            if (hp == 0)
            {
                //Debug.Log("Á×À½");
            }
        }
    }
    public int Gold = 0;
    private void Start()
    {
        mhp = int.MaxValue;
        HP = int.MaxValue;
        cards = new();
        foreach (var card in GameManager.Inst.cardJson)
        {
            cards.Add(card["id"].ToString());
        }

        GameManager.Inst.battle.SetRandomCard();
        GameManager.Inst.battle.SetRandomShopCard();
    }
}
