using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : Character
{
    public string id = string.Empty;
    public Transform HPBar;
    private TextMeshPro HPValue;
    public int mhp = 100;
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, mhp);
            HPBar.localScale = new((float)hp/mhp, 1, 1);
            HPValue.text = $"{hp} / {mhp}";
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
    protected override void Awake()
    {
        base.Awake();
        HPValue = GetComponentInChildren<TextMeshPro>();
    }
    private void Start()
    {
        HPBar.parent.localPosition = new(-0.75f, Mathf.Max(1, (GetComponent<SpriteRenderer>().sprite.texture.height - 150.0f)/100.0f));
        TurnUseCard = cards[Random.Range(0, cards.Count)];
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
        GameManager.Inst.battle.SetEff(FindCard(TurnUseCard).eff);
        GameManager.Inst.battle.TurnEnd();
    }
}
