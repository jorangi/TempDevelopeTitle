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

    protected virtual void Awake()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(Enums.effType)).Length; i++)
        {
            takeEff.Add(new(1, 0, (Enums.effType)i));
            giveEff.Add(new(1, 0, (Enums.effType)i));
            effResist.Add(new(0, 0, (Enums.effType)i));
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
    public string[] FindCard(params string[] tag)
    {
        List<string> tempCard = new();
        foreach (var card in GameManager.Inst.cardJson)
        {
            if(card["id"].ToString() == tag[0].ToString())
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
}
