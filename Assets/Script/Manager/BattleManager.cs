using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public GameObject CardUI;
    public GameObject ShopUI;
    public GameObject EnemyInfo;

    private int index = 0;
    private int mana = 0;
    private int gold = 0;

    public int Index
    {
        get => index;
        set
        {
            if (value > order.Count - 1)
            {
                for(int i = 0; i < order.Count; i++)
                {
                    if (order[i].hp > 0)
                    {
                        value = i;
                        break;
                    }
                }
                if(value == 0)
                {
                    SetRandomCard();
                }
                else if(value == order.Count)
                {
                    return;
                }
            }
            Target = null;
            index = value;
            Caster = order[index];
            if (Caster.hp == 0)
            {
                Index++;
                return;
            }
            else
            {
                TurnStart();
            }
            if (index != 0)
            {
                CardUI.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
                CardUI.transform.parent.Find("DisinteractPanel").SetAsLastSibling();
                CardUI.transform.parent.Find("DisinteractPanel").gameObject.SetActive(true);
                Target = GameManager.Inst.player;
                order[index].OnTurn();
            }
            else
            {
                CardUI.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
                CardUI.transform.parent.Find("DisinteractPanel").gameObject.SetActive(false);
            }
        }
    }
    public List<Character> order;
    public Character Caster, Target;
    public int Turn = 0;

    public TextMeshProUGUI ManaText, GoldText, HPText;
    private int maxMana = 0;
    public int Mana
    {
        get => mana;
        set
        {
            mana = value;
            ManaText.text = $"{value}/{maxMana}";
        }
    }
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            GoldText.text = $"{value} G";
        }
    }
    public bool cardDragging = false;

    private void Awake()
    {
        Turn = 0;
    }
    private void Start()
    {
        order = FindObjectsOfType<Character>().ToList();
        Index = 0;
        Mana = maxMana;
        Gold = 0;
        GameManager.Inst.player.HP = GameManager.Inst.player.mhp;
    }
    public void SetRandomCard()
    {
        List<string> list = new();
        foreach (string card in GameManager.Inst.player.cards)
        {
            list.Add(card);
        }
        for (int i = 0; i < CardUI.transform.childCount; i++)
        {
            if (list.Count == 0)
            {
                CardUI.transform.GetChild(i).gameObject.SetActive(false);
                return;
            }
            int rand = UnityEngine.Random.Range(0, list.Count);
            CardUI.transform.GetChild(i).GetComponent<CardData>().SetCardData(list[rand]);
            foreach (var card in GameManager.Inst.cardJson)
            {
                if (card["id"].ToString() == list[rand] && Array.IndexOf(card["category"].ToObject<string[]>(), "basic") == -1)
                {
                    GameManager.Inst.player.cards.Remove(list[rand]);
                }
            }
            list.RemoveAt(rand);
        }
    }
    public void SetRandomShopCard()
    {
        Transform ShopCards = ShopUI.transform.Find("CardShop");
        for (int i = 0; i < ShopCards.childCount; i++)
        {
            ShopCards.GetChild(i).GetComponent<CardData>().SetCardData(GameManager.Inst.cardJson[UnityEngine.Random.Range(0, GameManager.Inst.cardJson.Count)]["id"].ToString());
        }
    }
    public void Immediately() 
    {
        foreach (var data in order)
        {
            foreach (var Eff in data.myEff)
            {
                if (Eff.moment == Enums.moment.Immediately)
                {
                    ExcuteEff(data, Eff, Eff.moment);
                }
            }
        }

        RemoveEff();
    }
    public void TurnStart()
    {
        if(Caster.CompareTag("Player"))
        {
            maxMana = Mathf.Min(++maxMana, 10);
            Mana = maxMana;
        }
        foreach (var data in order)
        {
            foreach (var Eff in data.myEff)
            {
                if (Eff.moment == Enums.moment.TurnStart)
                {
                    ExcuteEff(data, Eff, Eff.moment);
                }
            }
        }

        RemoveEff();
    }
    public void TurnEnd()
    {
        foreach (var data in order)
        {
            foreach (var Eff in data.myEff)
            {
                if (Eff.moment == Enums.moment.TurnEnd)
                {
                    ExcuteEff(data, Eff, Eff.moment);
                }
                if(Eff.turn > 0)
                {
                    if(data.GetComponent<Player>() != null)
                    {
                        var target = data.GetComponent<Player>();
                        switch(Eff.eff)
                        {
                            case Enums.eff.Burn:
                                target.HP -= (int)(3 * target.takeEff[(int)Enums.effType.Burn].per * Eff.Caster.giveEff[(int)Enums.effType.Burn].per) + target.takeEff[(int)Enums.effType.Burn].add + Eff.Caster.giveEff[(int)Enums.effType.Burn].add;
                                break;
                            case Enums.eff.Poison:
                                target.HP -= (int)(2 * target.takeEff[(int)Enums.effType.Poison].per * Eff.Caster.giveEff[(int)Enums.effType.Burn].per) + target.takeEff[(int)Enums.effType.Poison].add + Eff.Caster.giveEff[(int)Enums.effType.Burn].add;
                                break;
                        }
                    }
                    else
                    {
                        var target = data.GetComponent<Enemy>();
                        switch (Eff.eff)
                        {
                            case Enums.eff.Burn:
                                target.HP -= (int)(3 * target.takeEff[(int)Enums.effType.Burn].per * Eff.Caster.giveEff[(int)Enums.effType.Burn].per) + target.takeEff[(int)Enums.effType.Burn].add + Eff.Caster.giveEff[(int)Enums.effType.Burn].add;
                                break;
                            case Enums.eff.Poison:
                                target.HP -= (int)(2 * target.takeEff[(int)Enums.effType.Poison].per * Eff.Caster.giveEff[(int)Enums.effType.Poison].per) + target.takeEff[(int)Enums.effType.Poison].add + Eff.Caster.giveEff[(int)Enums.effType.Poison].add;
                                break;
                        }
                    }
                }
                Eff.turn = Mathf.Max(--Eff.turn, 0);
            }
        }

        RemoveEff();
        Index++;
    }
    public void SetEff(string[] effs)
    {
        foreach(var eff in effs)
        {
            string[] effItem = eff.Split(' ');

            float resist = 0.0f;
            if (effItem[0] == "Target") // 지정한 단일 대상
            {
                bool haveEff = false;
                foreach (var Eff in Target.effResist)
                {
                    if (Eff.effType == (Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])))
                    {
                        resist = Eff.per;
                        break;
                    }
                }
                foreach(var Eff in Target.myEff)
                {
                    if (!(Eff.eff == Enums.eff.Damage || Eff.eff == Enums.eff.Heal || Eff.eff == Enums.eff.Mana || Eff.eff == Enums.eff.Inflict || Eff.eff == Enums.eff.Receive)) // 중첩이 되는에 한해서만 Eff
                    {
                        if (Eff.eff == (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1])) // 이미 보유한 Eff
                        {
                            haveEff = true;
                            if (effItem.Length == 4)
                            {
                                if (effItem[3].IndexOf("t") > -1)
                                {
                                    Eff.turn += Convert.ToInt32(effItem[3].Replace("t",""));
                                }
                                else
                                {
                                    Eff.accum += (int)(Convert.ToInt32(effItem[3]) * (1 - resist));
                                }
                            }
                            break;
                        }
                    }
                }
                if(!haveEff)
                {
                    //중첩이 되지 않는 Eff
                    if ((Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Damage || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Heal || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Mana || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Inflict || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Receive)
                    {
                        if (effItem.Length == 5)
                        {
                            Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[4]), 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                            Target.myEff.Add(_eff);
                        }
                        else if (effItem.Length == 4)
                        {
                            Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[3]), 0, 0, Caster);
                            Target.myEff.Add(_eff);
                        }
                    }
                    else // 중첩이 되는 Eff
                    {
                        if (effItem.Length == 5)
                        {
                            Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[4]) * (1 - resist)), Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                            Target.myEff.Add(_eff);
                        }
                        else if (effItem.Length == 4)
                        {
                            if (effItem[3].IndexOf('t') > -1)
                            {
                                Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                Target.myEff.Add(_eff);
                            }
                            else
                            {
                                Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[3]) * (1 - resist)), 0, Caster);
                                Target.myEff.Add(_eff);
                            }
                        }
                    }
                }
            }
            else if (effItem[0] == "Self")
            {
                bool haveEff = false;
                foreach (var Eff in Caster.effResist)
                {
                    if (Eff.effType == (Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])))
                    {
                        resist = Eff.per;
                        break;
                    }
                }
                foreach (var Eff in Caster.myEff)
                {
                    if (!(Eff.eff == Enums.eff.Damage || Eff.eff == Enums.eff.Heal || Eff.eff == Enums.eff.Mana || Eff.eff == Enums.eff.Inflict || Eff.eff == Enums.eff.Receive)) // 중첩이 되는에 한해서만 Eff
                    {
                        if (Eff.eff == (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1])) // 이미 보유한 Eff
                        {
                            haveEff = true;
                            if (effItem.Length == 4)
                            {
                                if (effItem[3].IndexOf("t") > -1)
                                {
                                    Eff.turn += Convert.ToInt32(effItem[3].Replace("t", ""));
                                }
                                else
                                {
                                    Eff.accum += (int)(Convert.ToInt32(effItem[3]) * (1 - resist));
                                }
                            }
                            break;
                        }
                    }
                }
                if (!haveEff)
                {
                    //중첩이 되지 않는 Eff
                    if ((Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Damage || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Heal || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Mana || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Inflict || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Receive)
                    {
                        if (effItem.Length == 5)
                        {
                            Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[4]), 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                            Caster.myEff.Add(_eff);
                        }
                        else if (effItem.Length == 4)
                        {
                            Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[3]), 0, 0, Caster);
                            Caster.myEff.Add(_eff);
                        }
                    }
                    else // 중첩이 되는 Eff
                    {
                        if (effItem.Length == 5)
                        {
                            Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[4]) * (1 - resist)), Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                            Caster.myEff.Add(_eff);
                        }
                        else if (effItem.Length == 4)
                        {
                            if (effItem[3].IndexOf('t') > -1)
                            {
                                Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                Caster.myEff.Add(_eff);
                            }
                            else
                            {
                                Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[3]) * (1 - resist)), 0, Caster);
                                Caster.myEff.Add(_eff);
                            }
                        }
                    }
                }
            }
            else if (effItem[0] == "Enemies")
            {
                if (Caster.gameObject.CompareTag("Player"))
                {
                    GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
                    for(int i = 0; i<targets.Length; i++)
                    {
                        Target = targets[i].GetComponent<Character>();
                        bool haveEff = false;

                        foreach (var Eff in Target.effResist)
                        {
                            if (Eff.effType == (Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])))
                            {
                                resist = Eff.per;
                                break;
                            }
                        }
                        foreach (var Eff in Target.myEff)
                        {
                            if (!(Eff.eff == Enums.eff.Damage || Eff.eff == Enums.eff.Heal || Eff.eff == Enums.eff.Mana || Eff.eff == Enums.eff.Inflict || Eff.eff == Enums.eff.Receive)) // 중첩이 되는에 한해서만 Eff
                            {
                                if (Eff.eff == (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1])) // 이미 보유한 Eff
                                {
                                    haveEff = true;
                                    if (effItem.Length == 4)
                                    {
                                        if (effItem[3].IndexOf("t") > -1)
                                        {
                                            Eff.turn += Convert.ToInt32(effItem[3].Replace("t", ""));
                                        }
                                        else
                                        {
                                            Eff.accum += (int)(Convert.ToInt32(effItem[3]) * (1 - resist));
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        if (!haveEff)
                        {
                            //중첩이 되지 않는 Eff
                            if ((Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Damage || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Heal || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Mana || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Inflict || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Receive)
                            {
                                if (effItem.Length == 5)
                                {
                                    Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[4]), 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                    Target.myEff.Add(_eff);
                                }
                                else if (effItem.Length == 4)
                                {
                                    Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[3]), 0, 0, Caster);
                                    Target.myEff.Add(_eff);
                                }
                            }
                            else // 중첩이 되는 Eff
                            {
                                if (effItem.Length == 5)
                                {
                                    Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[4]) * (1 - resist)), Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                    Target.myEff.Add(_eff);
                                }
                                else if (effItem.Length == 4)
                                {
                                    if (effItem[3].IndexOf('t') > -1)
                                    {
                                        Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                        Target.myEff.Add(_eff);
                                    }
                                    else
                                    {
                                        Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[3]) * (1 - resist)), 0, Caster);
                                        Target.myEff.Add(_eff);
                                    }
                                }
                            }
                        }
                    }
                }
                else 
                {
                    Target = GameManager.Inst.player;
                    bool haveEff = false;

                    foreach (var Eff in Target.effResist)
                    {
                        if (Eff.effType == (Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])))
                        {
                            resist = Eff.per;
                            break;
                        }
                    }
                    foreach (var Eff in Target.myEff)
                    {
                        if (!(Eff.eff == Enums.eff.Damage || Eff.eff == Enums.eff.Heal || Eff.eff == Enums.eff.Mana || Eff.eff == Enums.eff.Inflict || Eff.eff == Enums.eff.Receive)) // 중첩이 되는에 한해서만 Eff
                        {
                            if (Eff.eff == (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1])) // 이미 보유한 Eff
                            {
                                haveEff = true;
                                if (effItem.Length == 5)
                                {
                                    Eff.accum += (int)(Convert.ToSingle(effItem[4]) * (1 - resist));
                                }
                                else if (effItem.Length == 4)
                                {
                                    if (effItem[3].IndexOf("t") > -1)
                                    {
                                        Eff.turn += Convert.ToInt32(effItem[3].Replace("t", ""));
                                    }
                                    else
                                    {
                                        Eff.accum += (int)(Convert.ToInt32(effItem[3]) * (1 - resist));
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (!haveEff)
                    {
                        //중첩이 되지 않는 Eff
                        if ((Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Damage || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Heal || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Mana || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Inflict || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Receive)
                        {
                            if (effItem.Length == 5)
                            {
                                Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[4]), 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                Target.myEff.Add(_eff);
                            }
                            else if (effItem.Length == 4)
                            {
                                Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[3]), 0, 0, Caster);
                                Target.myEff.Add(_eff);
                            }
                        }
                        else // 중첩이 되는 Eff
                        {
                            if (effItem.Length == 4)
                            {
                                if (effItem[3].IndexOf('t') > -1)
                                {
                                    Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, 0, Convert.ToInt32(effItem[3].Replace("t", "")), Caster);
                                    Target.myEff.Add(_eff);
                                }
                                else
                                {
                                    Eff _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, (int)(Convert.ToInt32(effItem[3]) * (1 - resist)), 0, Caster);
                                    Target.myEff.Add(_eff);
                                }
                            }
                        }
                    }
                }
            }
        }
        Immediately();
    }
    private string FindEffType(string id)
    {
        foreach(var eff in GameManager.Inst.effJson)
        {
            if (eff["id"].ToString() == id)
            {
                return eff["effType"].ToString();
            }
        }
        return null;
    }
    private void ExcuteEff(Character _target, Eff eff, Enums.moment moment = Enums.moment.Immediately)
    {
        Debug.Log(_target);
        Debug.Log(eff.run);
        Debug.Log(eff.eff == Enums.eff.Heal);
        if (eff.run && (eff.eff == Enums.eff.Damage || eff.eff == Enums.eff.Heal || eff.eff == Enums.eff.Mana || eff.eff == Enums.eff.Inflict || eff.eff == Enums.eff.Receive))
            return;
        if(_target.GetComponent<Player>() != null)
        {
            var target = _target.GetComponent<Player>();
            if(eff.eff == Enums.eff.Damage)
            {
                target.HP -= (int)(eff.val * (target.takeEff[(int)Enums.effType.Damage].per) * (eff.Caster.giveEff[(int)Enums.effType.Damage].per)) + target.takeEff[(int)Enums.effType.Damage].add+ eff.Caster.giveEff[(int)Enums.effType.Damage].add;
            }
            if(eff.eff == Enums.eff.Heal)
            {
                target.HP += (int)(eff.val * (target.takeEff[(int)Enums.effType.Heal].per) * (eff.Caster.giveEff[(int)Enums.effType.Heal].per)) + target.takeEff[(int)Enums.effType.Heal].add + eff.Caster.giveEff[(int)Enums.effType.Heal].add;
            }
            if (eff.eff == Enums.eff.Mana)
            {
                mana += eff.val;
            }
            if (eff.eff == Enums.eff.Inflict)
            {
                target.giveEff[(int)Enums.effType.Damage].per += eff.val * 0.01f;
            }
            if(eff.eff == Enums.eff.Receive)
            {
                target.takeEff[(int)Enums.effType.Damage].per += eff.val * 0.01f;
            }
            if(eff.eff == Enums.eff.Burn)
            {
                while (eff.accum >= 100)
                {
                    eff.accum -= 100;
                    eff.turn++;
                }
                if(!eff.run)
                {
                    target.takeEff[(int)Enums.effType.Damage].per += 0.1f;
                }
            }
            if (eff.eff == Enums.eff.Poison)
            {
                while (eff.accum >= 100)
                {
                    eff.accum -= 100;
                    eff.turn++;
                }
                if (!eff.run)
                {
                    target.giveEff[(int)Enums.effType.Damage].per -= 0.1f;
                }
            }
        }
        else
        {
            var target = _target.GetComponent<Enemy>();
            if (target.isDead)
            {
                eff.run = true;
                return;
            }

            if (eff.eff == Enums.eff.Damage)
            {
                target.HP -= (int)(eff.val * (target.takeEff[(int)Enums.effType.Damage].per) * (eff.Caster.giveEff[(int)Enums.effType.Damage].per)) + target.takeEff[(int)Enums.effType.Damage].add + eff.Caster.giveEff[(int)Enums.effType.Damage].add;
            }
            if (eff.eff == Enums.eff.Heal)
            {
                target.HP += (int)(eff.val * (target.takeEff[(int)Enums.effType.Heal].per) * (eff.Caster.giveEff[(int)Enums.effType.Heal].per)) + target.takeEff[(int)Enums.effType.Heal].add + eff.Caster.giveEff[(int)Enums.effType.Heal].add;
            }
            if (eff.eff == Enums.eff.Inflict)
            {
                target.giveEff[(int)Enums.effType.Damage].per += eff.val * 0.01f;
            }
            if (eff.eff == Enums.eff.Receive)
            {
                target.takeEff[(int)Enums.effType.Damage].per += eff.val * 0.01f;
            }
            if (eff.eff == Enums.eff.Burn)
            {
                while (eff.accum >= 100)
                {
                    eff.accum -= 100;
                    eff.turn++;
                }
                if (!eff.run)
                {
                    target.takeEff[(int)Enums.effType.Damage].per += 0.1f;
                }
            }
            if (eff.eff == Enums.eff.Poison)
            {
                while (eff.accum >= 100)
                {
                    eff.accum -= 100;
                    eff.turn++;
                }
                if (!eff.run)
                {
                    target.giveEff[(int)Enums.effType.Damage].per -= 0.1f;
                }
            }
        }
        eff.run = true;
    }
    private void RemoveEff()
    {
        foreach (var data in order)
        {
            for (int i = data.myEff.Count - 1; i >= 0; i--)
            {
                if (data.myEff[i].run && data.myEff[i].turn == 0)
                {
                    if(data.GetComponent<Player>()!=null)
                    {
                        var target = data.GetComponent<Player>();
                        if (data.myEff[i].eff == Enums.eff.Inflict)
                        {
                            target.giveEff[(int)Enums.effType.Damage].per -= data.myEff[i].val * 0.01f;
                        }
                        if (data.myEff[i].eff == Enums.eff.Receive)
                        {
                            target.takeEff[(int)Enums.effType.Damage].per -= data.myEff[i].val * 0.01f;
                        }
                        if (data.myEff[i].eff == Enums.eff.Burn)
                        {
                            target.takeEff[(int)Enums.effType.Damage].per -= 0.1f;
                        }
                        if (data.myEff[i].eff == Enums.eff.Poison)
                        {
                            target.giveEff[(int)Enums.effType.Damage].per += 0.1f;
                        }
                    }
                    else
                    {
                        var target = data.GetComponent<Enemy>();
                        if (data.myEff[i].eff == Enums.eff.Inflict)
                        {
                            target.giveEff[(int)Enums.effType.Damage].per -= data.myEff[i].val * 0.01f;
                        }
                        if (data.myEff[i].eff == Enums.eff.Receive)
                        {
                            target.takeEff[(int)Enums.effType.Damage].per -= data.myEff[i].val * 0.01f;
                        }
                        if (data.myEff[i].eff == Enums.eff.Burn)
                        {
                            target.takeEff[(int)Enums.effType.Damage].per -= 0.1f;
                        }
                        if (data.myEff[i].eff == Enums.eff.Poison)
                        {
                            target.giveEff[(int)Enums.effType.Damage].per += 0.1f;
                        }
                    }
                    data.myEff[i].run = false;
                    if (data.myEff[i].eff == Enums.eff.Damage || data.myEff[i].eff == Enums.eff.Heal || data.myEff[i].eff == Enums.eff.Mana|| data.myEff[i].eff == Enums.eff.Inflict|| data.myEff[i].eff == Enums.eff.Receive)
                    {
                        data.myEff.RemoveAt(i);
                    }
                }
            }
        }
    }
    public void ShowEnemyInfo(Character data)
    {
        EnemyInfo.SetActive(true);
        EnemyInfo.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition.x < (Screen.width / 3) * 2? ((Vector2)Input.mousePosition * new Vector2(1920.0f / Screen.width, 0) + new Vector2(150.0f, 0)) : ((Vector2)Input.mousePosition  * new Vector2(1920.0f / Screen.width, 0) + new Vector2(-450.0f, 0));
        EnemyInfo.transform.Find("Inflict").transform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{data.giveEff[(int)Enums.effType.Damage].per * 100.0f}% + {data.giveEff[(int)Enums.effType.Damage].add}";
        EnemyInfo.transform.Find("Receive").transform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{data.takeEff[(int)Enums.effType.Damage].per * 100.0f}% + {data.takeEff[(int)Enums.effType.Damage].add}";
        EnemyInfo.transform.Find("Burn").transform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{data.effResist[(int)Enums.effType.Burn].per * 100.0f}% | {data.takeEff[(int)Enums.effType.Burn].per * 100.0f}% + {data.takeEff[(int)Enums.effType.Burn].add}";
        EnemyInfo.transform.Find("Poison").transform.Find("Value").GetComponent<TextMeshProUGUI>().text = $"{data.effResist[(int)Enums.effType.Poison].per * 100.0f}% | {data.takeEff[(int)Enums.effType.Poison].per * 100.0f}% + {data.takeEff[(int)Enums.effType.Poison].add}";
    }
}
