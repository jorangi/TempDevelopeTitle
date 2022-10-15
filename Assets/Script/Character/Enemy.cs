using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : Character
{
    public string id = string.Empty;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        TurnUseCard = cards[Random.Range(0, cards.Count)];
    }
    public override void OnTurn()
    {
        base.OnTurn();
        StartCoroutine(CardUse());
    }
    private IEnumerator CardUse()
    {
        GameObject obj = Instantiate(GameManager.Inst.battle.CardPrefab, GameManager.Inst.battle.UseCardUI.transform);
        obj.GetComponent<CardData>().SetCardData(TurnUseCard);
        obj.GetComponent<Image>().raycastTarget = false;
        obj.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(transform.position);
        obj.GetComponent<CardData>().ReplaceDesc(GameManager.Inst.player);
        yield return StartCoroutine(obj.GetComponent<CardData>().UseCard());
        yield return new WaitForSeconds(1);
        GameManager.Inst.battle.SetEff(FindCard(TurnUseCard).eff);
        GameManager.Inst.battle.TurnEnd();
        Destroy(obj);
    }
}
