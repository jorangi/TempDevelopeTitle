using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class CardDetail : MonoBehaviour
{
    public GameObject DataDescBox;
    public Card data;
    public TextMeshProUGUI Mana, Name, Desc;
    public Image cardImage, image;
    public string id;
    public string[] availableCategory;

    private BattleManager battle;
    private void Awake()
    {
        GetComponent<Image>().raycastTarget = false;
    }
    public void SetCardData(string id)
    {
        battle = GameManager.Inst.battle;
        availableCategory = new string[] { "burn", "poison", "mana" };
        JArray cards = GameManager.Inst.cardJson;
        foreach (JObject card in cards)
        {
            if (card["id"].ToString() == id)
            {
                this.id = id;
                data = new()
                {
                    id = id,
                    cardName = card["name"].Value<string>(),
                    cardDesc = card["desc"].ToString(),
                    category = card["category"].ToObject<string[]>(),
                    synergy = card["synergy"].ToObject<string[]>(),
                    eff = card["eff"].ToObject<string[]>(),
                    mana = card["mana"].ToObject<int>()
                };

                image.sprite = Resources.Load<Sprite>($"Images/Card/{id}");
                cardImage.sprite = Resources.Load<Sprite>($"Images/Card/{data.category[0]}Card");
                DisplayData();
                return;
            }
        }
    }
    private void DisplayData()
    {
        Mana.text = data.mana.ToString();
        Name.text = data.cardName;
        Desc.text = data.cardDesc;

        ReplaceDesc(null);

        Transform Descs = transform.parent.Find("Descs");
        Descs.gameObject.SetActive(false);
        for (int i = Descs.childCount - 1; i >= 0; i--)
        {
            Destroy(Descs.GetChild(i).gameObject);
        }
        foreach (string categoryItem in data.category)
        {
            if(Array.IndexOf(availableCategory, categoryItem) > -1)
            {
                Descs.gameObject.SetActive(true);
                foreach (var Item in GameManager.Inst.effJson)
                {
                    if (Item["id"].ToString().ToLower() == categoryItem)
                    {
                        GameObject obj = Instantiate(DataDescBox, Descs);
                        obj.transform.Find("EffName").GetComponent<TextMeshProUGUI>().text = Item["name"].ToString();
                        obj.transform.Find("EffDesc").GetComponent<TextMeshProUGUI>().text = Item["desc"].ToString();
                    }
                }
            }
        }
    }
    public void ReplaceDesc(Character _target)
    {
        if (data.cardDesc.IndexOf('[') > -1)
        {
            string convertDesc = "";
            string[] tempString = data.cardDesc.Split('[', ']');
            for (int i = 0; i < tempString.Length; i++)
            {
                if (i % 2 == 1)
                {
                    foreach (var eff in GameManager.Inst.effJson)
                    {
                        if (eff["id"].ToString().ToLower() == tempString[i])
                        {
                            tempString[i] = eff["name"].ToString().ToUpper();
                        }
                    }
                }
            }
            foreach (string temp in tempString)
            {
                convertDesc += temp;
            }
            Desc.text = convertDesc;
        }
        if (Desc.text.IndexOf("{") > -1)
        {
            string convertDesc = "";
            string[] tempString = Desc.text.Split('{', '}');
            for (int i = 0; i < tempString.Length; i++)
            {
                if (i % 2 == 1)
                {
                    if (data.eff[Convert.ToInt32(tempString[i].Split('_')[0])].Split(' ')[3].IndexOf("t") == -1)
                    {
                        float resist = 0.0f;

                        string[] effItem = data.eff[Convert.ToInt32(tempString[i].Split('_')[0])].Split(' ');


                        if (_target != null && effItem[0] == "Target")
                        {
                            foreach (var Eff in _target.effResist)
                            {
                                if (Eff.effType == (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])))
                                {
                                    resist = Eff.per;
                                    break;
                                }
                            }
                        }
                        else if (effItem[0] == "Self")
                        {
                            foreach (var Eff in battle.Caster.effResist)
                            {
                                if (Eff.effType == (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])))
                                {
                                    resist = Eff.per;
                                    break;
                                }
                            }
                        }
                        Eff _eff = null;
                        if ((Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Damage || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Heal || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Mana || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Inflict || (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]) == Enums.eff.Receive)
                        {
                            if (effItem.Length == 5)
                            {
                                _eff = new(data.id, (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[4]), 0, Convert.ToInt32(effItem[3].Replace("t", "")), battle.Caster);
                            }
                            else if (effItem.Length == 4)
                            {
                                _eff = new(data.id, (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[3]), 0, 0, battle.Caster);
                            }
                        }
                        else
                        {
                            if (effItem.Length == 5)
                            {
                                _eff = new(data.id, (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, Mathf.RoundToInt(Convert.ToInt32(effItem[4]) * (1 - resist)), Convert.ToInt32(effItem[3].Replace("t", "")), battle.Caster);
                            }
                            else if (effItem.Length == 4)
                            {
                                if (effItem[3].IndexOf('t') > -1)
                                {
                                    _eff = new(data.id, (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, 0, Convert.ToInt32(effItem[3].Replace("t", "")), battle.Caster);
                                }
                                else
                                {
                                    _eff = new(data.id, (Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, Mathf.RoundToInt(Convert.ToInt32(effItem[3]) * (1 - resist)), 0, battle.Caster);
                                }
                            }
                        }
                        tempString[i] = CalcValue(_target, _eff).ToString();
                    }
                    else
                    {
                        tempString[i] = data.eff[Convert.ToInt32(tempString[i].Split('_')[0])].Split(' ')[Convert.ToInt32(tempString[i].Split('_')[1])];
                    }
                    tempString[i] = tempString[i].Replace("t", "").Replace("-", "");
                }
            }
            foreach (string temp in tempString)
            {
                convertDesc += temp;
            }
            Desc.text = convertDesc;
        }
    }
    public int CalcValue(Character target, Eff eff)
    {
        int val = 0;
        if (target == null)
        {
            if (eff.eff == Enums.eff.Damage)
            {
                val = (int)(eff.val * eff.Caster.giveEff[(int)Enums.effType.Damage].per + eff.Caster.giveEff[(int)Enums.effType.Damage].add);
            }
            if (eff.eff == Enums.eff.Heal)
            {
                val = (int)(eff.val * eff.Caster.giveEff[(int)Enums.effType.Heal].per + eff.Caster.giveEff[(int)Enums.effType.Heal].add);
            }
            if (eff.eff == Enums.eff.Mana || eff.eff == Enums.eff.Inflict || eff.eff == Enums.eff.Receive)
            {
                val = eff.val;
            }
            if (eff.eff == Enums.eff.Burn || eff.eff == Enums.eff.Poison)
            {
                val = eff.accum;
            }
        }
        else
        {
            if (eff.eff == Enums.eff.Damage)
            {
                val = (int)(eff.val * target.takeEff[(int)Enums.effType.Damage].per * (eff.Caster.giveEff[(int)Enums.effType.Damage].per)) + target.takeEff[(int)Enums.effType.Damage].add + eff.Caster.giveEff[(int)Enums.effType.Damage].add;
            }
            if (eff.eff == Enums.eff.Heal)
            {
                val = (int)(eff.val * target.takeEff[(int)Enums.effType.Heal].per * (eff.Caster.giveEff[(int)Enums.effType.Heal].per)) + target.takeEff[(int)Enums.effType.Heal].add + eff.Caster.giveEff[(int)Enums.effType.Heal].add;
            }
            if (eff.eff == Enums.eff.Mana || eff.eff == Enums.eff.Inflict || eff.eff == Enums.eff.Receive)
            {
                val = eff.val;
            }
            if (eff.eff == Enums.eff.Burn || eff.eff == Enums.eff.Poison)
            {
                val = eff.accum;
            }
        }
        return val;
    }
}
