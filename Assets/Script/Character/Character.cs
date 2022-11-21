using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{
    public GameObject Info;

    public Vector2Int pos;

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
                //Debug.Log("죽음");
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
            subhp = Mathf.Clamp(value, 0, mhp);
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

    private TextMeshProUGUI HPValue;

    public string[] category;

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
        category = new string[] { "가", "나", "다", "라", "마", "바", "사"};
    }
    protected virtual void Start()
    {
        //HPBar.parent.localPosition = new(-0.75f, Mathf.Max(0.7f, (float)(GetComponent<SpriteRenderer>().sprite.texture.height - 100.0f) / 80.0f));
        //Targeting.transform.localPosition = new(0, Mathf.Max(1.5f, (float)(GetComponent<SpriteRenderer>().sprite.texture.height - 100.0f) / 80.0f + 0.8f));
    }
    public void Initialize()
    {
        HPBar = Info.transform.Find("HPBack").Find("HPBar");
        SubHPBar = Info.transform.Find("HPBack").Find("SubHPBar");
        HPValue = Info.transform.Find("HPBack").GetComponentInChildren<TextMeshProUGUI>();

        if (category.Length > 5)
        {
            GameObject cat = Instantiate(GameManager.Inst.battle.CategoryInInfo, Info.transform.Find("Categories"));
            cat.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            cat.name = "category";
            cat.transform.localPosition = new(43 * 6, 0, 0);
            cat.transform.Find("Mask").Find("Count").gameObject.SetActive(true);
            cat.transform.Find("Mask").Find("Count").GetComponent<TextMeshProUGUI>().text = $"+{(category.Length - 6)}";
        }

        for (int i = 0; i<category.Length; i++)
        {
            if (i > 5)
                break;
            GameObject cat = Instantiate(GameManager.Inst.battle.CategoryInInfo, Info.transform.Find("Categories"));
            cat.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            cat.name = "category";
            cat.transform.localPosition = new(43 * i, 0, 0);
            cat.transform.Find("Mask").Find("Image").gameObject.SetActive(true);
            cat.transform.Find("Mask").Find("Count").gameObject.SetActive(true);
            cat.transform.Find("Mask").Find("Count").GetComponent<TextMeshProUGUI>().text = category[i];
        }
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
        //GameManager.Inst.battle.EnemyInfo.SetActive(false);
    }
    private IEnumerator HPBarLerp()
    {
        //Last Sibling : 더 위에 보임
        //SubBar : 먼저 잘리는 것
        //HpBar : 천천히 따라오는 것
        Image hpBar = HPBar.GetComponent<Image>();
        Image SubhpBar = SubHPBar.GetComponent<Image>();
        if (mhp == 0)
            yield break;
        if (hp > subhp) //체력 증가
        {
            HPBar.transform.SetAsFirstSibling();
            HPBar.GetComponent<Image>().color = new(0.77f, 0.55f, 0.55f);
            SubHPBar.GetComponent<Image>().color = Color.white;
        }
        else //체력 감소
        {
            SubHPBar.transform.SetAsFirstSibling();
            HPBar.GetComponent<Image>().color = Color.white;
            SubHPBar.GetComponent<Image>().color = new(0.77f, 0.55f, 0.55f);
        }
        while (Mathf.Abs(hpBar.fillAmount - (float)hp / mhp) > 0.01f)
        {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, (float)hp / mhp, Time.deltaTime * 7);
            yield return null;
        }
        hpBar.fillAmount = (float)hp / mhp;

        while (Mathf.Abs(SubhpBar.fillAmount - (float)hp / mhp) > 0.01f)
        {
            SubhpBar.fillAmount = Mathf.Lerp(SubhpBar.fillAmount, (float)hp / mhp, Time.deltaTime * 15);
            yield return null;
        }
        SubHP = HP;
        SubhpBar.fillAmount = (float)hp / mhp;
    }
}
