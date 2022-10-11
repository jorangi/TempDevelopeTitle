using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.EventSystems;

public class Card
{
    public string cardName, cardDesc;
    public string[] category, synergy, eff;
    public int mana, gold;
}
public class CardData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerClickHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 startPoint, moveBegin, moveOffset;
    private Image img;

    public Card data;
    public TextMeshProUGUI Mana, Gold, Name, Desc;
    public Image card, image;
    public string id;

    private bool IsUse;
    private bool Dragging = false;
    private bool IsshopCard = false;
    private GameObject tempObj = null;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        IsshopCard = transform.Find("Gold");
    }
    public void SetCardData(string id)
    {
        JArray cards = GameManager.Inst.cardJson;
        foreach (JObject card in cards)
        {
            if (card["id"].ToString() == id)
            {
                this.id = id;
                data = new()
                {
                    cardName = card["name"].Value<string>(),
                    cardDesc = card["desc"].ToString(),
                    category = card["category"].ToObject<string[]>(),
                    synergy = card["synergy"].ToObject<string[]>(),
                    eff = card["eff"].ToObject<string[]>(),
                    mana = card["mana"].ToObject<int>(),
                    gold = card["gold"].ToObject<int>()
                };

                image.sprite = Resources.Load<Sprite>($"Images/Card/{id}");
                DisplayData();
                return;
            }
        }
    }
    private void DisplayData()
    {
        if(Gold != null)
        {
            Mana.text = data.mana.ToString();
            Gold.text = $"{data.gold.ToString()} G";
            IsshopCard = true;
        }
        else
        {
            Mana.text = data.mana.ToString();
        }
        Name.text = data.cardName;
        Desc.text = data.cardDesc;
        if (data.cardDesc.IndexOf("{")>-1)
        {
            string convertDesc = "";
            string[] tempString = data.cardDesc.Split('{', '}');
            for (int i = 0; i < tempString.Length; i++)
            {
                if(i%2==1)
                {
                    tempString[i] = data.eff[Convert.ToInt32(tempString[i].Split('_')[0])].Split(' ')[Convert.ToInt32(tempString[i].Split('_')[1])];
                    tempString[i] = tempString[i].Replace("t", "").Replace("-", "");
                }
            }
            foreach(string temp in tempString)
            {
                convertDesc += temp;
            }
            Desc.text = convertDesc;
        }
        IsUse = false;
        image.color = new (1.0f, 1.0f, 1.0f);
        card.color = new(1.0f, 1.0f, 1.0f);
        img = null;
    }
    private IEnumerator ResetPosition()
    {
        while((rect.anchoredPosition - startPoint).sqrMagnitude > 0.01f)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, startPoint, Time.deltaTime * 10);
            yield return null;
        }
        rect.anchoredPosition = startPoint;
        Dragging = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Transform cardDetailBox = GameManager.Inst.battle.CardUI.transform.parent.parent.parent.Find("CardDetailBox");
            cardDetailBox.transform.Find("Card").GetComponent<CardDetail>().data = new()
            {
                cardDesc = data.cardDesc,
                cardName = data.cardName,
                category = data.category,
                mana = data.mana,
                eff = data.eff
            };
            cardDetailBox.SetAsLastSibling();
            cardDetailBox.gameObject.SetActive(true);
        }
        if (IsshopCard || Dragging || data.mana > GameManager.Inst.battle.Mana)
            return;
        startPoint = rect.anchoredPosition;
        moveBegin = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (IsshopCard || data.mana > GameManager.Inst.battle.Mana)
            return;
        GameManager.Inst.battle.cardDragging = true;
        if (!IsUse)
        {
            Cursor.SetCursor(GameManager.Inst.CursorImg.texture, Vector2.zero, CursorMode.ForceSoftware);
            Dragging = true;
            bool onSpawnable = false;
            RaycastHit2D hit = Physics2D.Raycast(eventData.pointerCurrentRaycast.worldPosition, transform.forward, 100.0f, LayerMask.GetMask("Field"));
            bool isTargetting = (hit.collider != null) && hit.collider.gameObject.CompareTag("Enemy") && (Array.IndexOf(data.category, "target")>-1);
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);
            moveOffset = (eventData.position  - moveBegin) * new Vector2(1920.0f / Screen.width, 1080.0f / Screen.height);
            rect.anchoredPosition = startPoint + moveOffset;
            canvasGroup.alpha = 1;
            if (isTargetting)
            {
                canvasGroup.alpha = 0;
                Cursor.SetCursor(GameManager.Inst.AttackCursor.texture, Vector2.zero, CursorMode.ForceSoftware);
            }
            foreach (var result in results)
            {
                if (result.gameObject.name == "SpawnSlot" && result.gameObject.GetComponent<SpawnSlotData>().data == null)
                {
                    onSpawnable = true;
                    if (img != null)
                    {
                        img.gameObject.SetActive(false);
                        img.sprite = null;
                        img.color = Color.white;
                    }
                    img = result.gameObject.transform.Find("illust").GetComponent<Image>();
                    img.gameObject.SetActive(true);
                    img.sprite = image.sprite;
                    tempObj = result.gameObject;
                    img.color = new Color(0.5f, 0.5f, 0.5f);
                    canvasGroup.alpha = 0;
                }
            }
            if (!onSpawnable && img != null && !tempObj.GetComponent<SpawnSlotData>().FilledSlot)
            {
                img.gameObject.SetActive(false);
                img.sprite = null;
                img.color = Color.white;
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsshopCard || data.mana > GameManager.Inst.battle.Mana)
            return;
        GameManager.Inst.battle.cardDragging = false;
        if (!IsUse)
        {
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);
            canvasGroup.alpha = 1;
            RaycastHit2D hit = Physics2D.Raycast(eventData.pointerCurrentRaycast.worldPosition, transform.forward, 100.0f, LayerMask.GetMask("Field"));
            bool isTargetting = (hit.collider != null) && hit.collider.gameObject.CompareTag("Enemy") && (Array.IndexOf(data.category, "target") > -1);
            StartCoroutine(ResetPosition());
            foreach (var result in results)
            {
                if (result.gameObject.name == "SpawnSlot" && result.gameObject.GetComponent<SpawnSlotData>().data == null)
                {
                    img.color = new(1, 1, 1);
                    result.gameObject.GetComponent<SpawnSlotData>().data = new()
                    {
                        synergy = data.synergy,
                    };
                    result.gameObject.GetComponent<SpawnSlotData>().FilledSlot = true;
                    image.color = new Color(0.5f, 0.5f, 0.5f);
                    card.color = new Color(0.5f, 0.5f, 0.5f);
                    IsUse = true;
                    GameManager.Inst.battle.Mana -= data.mana;
                }
            }
            if(isTargetting)
            {
                IsUse = true;
                image.color = new Color(0.5f, 0.5f, 0.5f);
                card.color = new Color(0.5f, 0.5f, 0.5f);
                GameManager.Inst.battle.Target = hit.collider.GetComponent<Character>();
                GameManager.Inst.battle.SetEff(data.eff);
                Cursor.SetCursor(GameManager.Inst.CursorImg.texture, Vector2.zero, CursorMode.ForceSoftware);
                GameManager.Inst.battle.Mana -= data.mana;
                return;
            }
            if(hit.collider != null && Array.IndexOf(data.category, "target") == -1)
            {
                IsUse = true;
                image.color = new Color(0.5f, 0.5f, 0.5f);
                card.color = new Color(0.5f, 0.5f, 0.5f);
                GameManager.Inst.battle.SetEff(data.eff);
                Cursor.SetCursor(GameManager.Inst.CursorImg.texture, Vector2.zero, CursorMode.ForceSoftware);
                GameManager.Inst.battle.Mana -= data.mana;
                return;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsshopCard && !IsUse)
        {
            if(data.gold <= GameManager.Inst.player.Gold)
            {
                GameManager.Inst.player.AddCard(id);
                image.color = new Color(0.5f, 0.5f, 0.5f);
                card.color = new Color(0.5f, 0.5f, 0.5f);
                IsUse = true;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(IsshopCard)
        {
            transform.parent.parent.SetAsLastSibling();
            transform.GetChild(0).localScale = new Vector3(2.85f, 2.85f, 1);
            if(rect.anchoredPosition.x == 855)
            {
                transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 0);
            }
            if(rect.anchoredPosition.y == -260)
            {
                transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            }
        }
        else
        {
            transform.parent.parent.SetAsLastSibling();
            transform.GetChild(0).localScale = new Vector3(2f, 2f, 1);
        }
        transform.SetAsLastSibling();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        if (rect.anchoredPosition.x == 855)
        {
            transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        if (rect.anchoredPosition.y == -260)
        {
            transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }
}