using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int hp = 100;
    public List<EffType> takeEff = new();
    public List<EffType> giveEff = new();
    public List<EffType> effResist = new();
    public List<Eff> myEff = new();
    public List<string> cards;
    public string TurnUseCard = string.Empty;

    protected virtual void Awake()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(Enums.effType)).Length; i++)
        {
            takeEff.Add(new(1, 0, (Enums.effType)i));
            giveEff.Add(new(1, 0, (Enums.effType)i));
            effResist.Add(new(0, 0, (Enums.effType)i));
        }
        cards = new List<string>();
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
}
