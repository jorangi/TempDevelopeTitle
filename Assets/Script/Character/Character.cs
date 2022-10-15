using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    public bool isDead = false;

    public int mhp = 100;
    public int hp = 100;
    public int subhp = 100;
    IEnumerator hpBarLerp = null;
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, mhp);
            if(hpBarLerp != null)
            {
                StopCoroutine(hpBarLerp);
            }
            StartCoroutine(HPBarLerp());
            HPValue.text = $"{hp} / {mhp}";
            if (hp == 0)
            {
                //Debug.Log("Á×À½");
                isDead = true;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
    public int SubHP
    {
        get => subhp;
        set
        {
            if(value > subhp)
            {
                HPBar.GetComponent<SpriteRenderer>().color = Color.white;
                HPBar.GetComponent<SpriteRenderer>().sortingOrder = -1;
                SubHPBar.GetComponent<SpriteRenderer>().color = new(0.77f, 0.55f, 0.55f);
                SubHPBar.GetComponent<SpriteRenderer>().sortingOrder = -2;
            }
            else
            {
                SubHPBar.GetComponent<SpriteRenderer>().color = Color.white;
                SubHPBar.GetComponent<SpriteRenderer>().sortingOrder = -1;
                HPBar.GetComponent<SpriteRenderer>().color = new(0.77f, 0.55f, 0.55f);
                HPBar.GetComponent<SpriteRenderer>().sortingOrder = -2;
            }
            subhp = Mathf.Clamp(value, 0, mhp);
            SubHPBar.localScale = new((float)subhp / mhp, 1, 1);
        }
    }

    public List<EffType> takeEff = new();
    public List<EffType> giveEff = new();
    public List<EffType> effResist = new();

    public List<Eff> myEff = new();

    public List<string> cards;

    public string TurnUseCard = string.Empty;

    public GameObject Targeting;

    public Transform HPBar;
    public Transform SubHPBar;

    private TextMeshPro HPValue;


    private float timer = 0;
    public float Timer
    {
        get => timer;
        set
        {
            timer = value;
            if (timer >= 0.75f)
            {
                GameManager.Inst.battle.ShowCharacterInfo(this);
            }
        }
    }

    protected virtual void Awake()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(Enums.effType)).Length; i++)
        {
            takeEff.Add(new(1, 0, (Enums.effType)i));
            giveEff.Add(new(1, 0, (Enums.effType)i));
            effResist.Add(new(0, 0, (Enums.effType)i));
        }
        cards = new List<string>();
        HPValue = GetComponentInChildren<TextMeshPro>();
    }
    protected virtual void Start()
    {
        HPBar.parent.localPosition = new(-0.75f, Mathf.Max(0.7f, (float)(GetComponent<SpriteRenderer>().sprite.texture.height - 100.0f) / 80.0f));
        Targeting.transform.localPosition = new(0, Mathf.Max(1.5f, (float)(GetComponent<SpriteRenderer>().sprite.texture.height - 100.0f) / 80.0f + 0.8f));
    }
    public void AddCard(string id)
    {
        foreach (var card in GameManager.Inst.cardJson)
        {
            if (card["id"].ToString() == id)
            {
                cards.Add(id);
                return;
            }
        }
    }
    public string[] FindCards(params string[] tag)
    {
        List<string> tempCard = new();
        foreach (var card in GameManager.Inst.cardJson)
        {
            if(System.Array.IndexOf(cards.ToArray(), card["id"].ToString())>-1)
            foreach(string tagItem in tag)
            {
                if (System.Array.IndexOf(card["category"].ToObject<string[]>(), tagItem) > -1)
                {
                    tempCard.Add(card["id"].ToString());
                    break;
                }
            }
        }
        return tempCard.ToArray();
    }
    public Card FindCard(string id)
    {
        Card data = new();
        foreach (var card in GameManager.Inst.cardJson)
        {
            if (card["id"].ToString() == id)
            {
                data.cardDesc = card["desc"].ToString();
                data.cardName = card["name"].ToString();
                data.mana = card["mana"].ToObject<int>();
                data.category = card["category"].ToObject<string[]>();
                data.eff = card["eff"].ToObject<string[]>();
                break;
            }
        }
        return data;
    }
    public void SetResist(string data)
    {
        string[] datas = data.Split(' ');
        foreach(var resist in effResist)
        {
            if(resist.effType.ToString() == datas[0])
            {
                resist.per = System.Convert.ToSingle(datas[1]);
                return;
            }
        }
    }
    public virtual void OnTurn()
    {
    }
    private void OnMouseOver()
    {
        if (!GameManager.Inst.battle.cardDragging)
            Timer += Time.deltaTime;
    }
    private void OnMouseExit()
    {
        Timer = 0;
        GameManager.Inst.battle.EnemyInfo.SetActive(false);
    }
    private IEnumerator HPBarLerp()
    {
        while(Mathf.Abs(HPBar.localScale.x - (float)hp / mhp) > 0.01f)
        {
            HPBar.localScale = new Vector3(Mathf.Lerp(HPBar.localScale.x, (float)hp / mhp, Time.deltaTime * 7), 1, 1);
            yield return null;
        }
        HPBar.localScale = new((float)hp / mhp, 1, 1);
        SubHP = HP;
    }
}
