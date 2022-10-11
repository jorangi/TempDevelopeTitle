using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public int mhp = 100;
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, mhp);
            if (hp == 0)
            {
                //Debug.Log("Á×À½");
                isDead = true;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
    public bool isDead = false;
    private float timer = 0;
    public float Timer
    {
        get => timer;
        set
        {
            timer = value;
            if (timer >= 0.75f)
            {
                GameManager.Inst.battle.ShowEnemyInfo(this);
            }
        }
    }
    private void Start()
    {
        cards = new();
        foreach (var card in GameManager.Inst.cardJson)
        {
            cards.Add(card["id"].ToString());
        }
    }
    private void OnMouseOver()
    {
        if(!GameManager.Inst.battle.cardDragging)
            Timer += Time.deltaTime;
    }
    private void OnMouseExit()
    {
        Timer = 0;
        GameManager.Inst.battle.EnemyInfo.SetActive(false);
    }
    public override void OnTurn()
    {
        base.OnTurn();
        string[] damageCards = FindCards("damage");
        GameManager.Inst.battle.SetEff(FindCard(damageCards[Random.Range(0, damageCards.Length)]).eff);
        GameManager.Inst.battle.TurnEnd();
    }
}
