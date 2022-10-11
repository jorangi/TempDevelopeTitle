using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDetail : MonoBehaviour
{
    public GameObject DataDescBox;
    public Card data;
    public TextMeshProUGUI Mana, Name, Desc;
    public Image card, image;
    public string id;
    public string[] availableCategory;

    private void Awake()
    {
        availableCategory = new string[] { "burn", "poison"};
    }
    private void OnEnable()
    {
        DisplayData();
    }
    private void DisplayData()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        Mana.text = data.mana.ToString();
        Name.text = data.cardName;
        Desc.text = data.cardDesc;
        if (data.cardDesc.IndexOf("{") > -1)
        {
            string convertDesc = "";
            string[] tempString = data.cardDesc.Split('{', '}');
            for (int i = 0; i < tempString.Length; i++)
            {
                if (i % 2 == 1)
                {
                    tempString[i] = data.eff[Convert.ToInt32(tempString[i].Split('_')[0])].Split(' ')[Convert.ToInt32(tempString[i].Split('_')[1])];
                    tempString[i] = tempString[i].Replace("t", "").Replace("-", "");
                }
            }
            foreach (string temp in tempString)
            {
                convertDesc += temp;
            }
            Desc.text = convertDesc;
        }

        Transform Descs = transform.parent.Find("Descs");

        for (int i = Descs.childCount - 1; i >= 0; i--)
        {
            Destroy(Descs.GetChild(i).gameObject);
        }

        foreach (string categoryItem in data.category)
        {
            if(Array.IndexOf(availableCategory, categoryItem) > -1)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(-450, 0);
                foreach(var Item in GameManager.Inst.effJson)
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
}
