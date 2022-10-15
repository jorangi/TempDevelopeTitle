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
public class CardData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IPointerDownHandler, IEndDragHandler, IPointerClickHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 startPoint, moveBegin, moveOffset;
    private Image img;

    public Card data;
    public TextMeshProUGUI Mana, Gold, Name, Desc;
    public Image cardImage, image;
    public string id;

    private Coroutine sizeChanger = null;
    private Coroutine useCard = null;

    private bool IsUse;
    private bool DragSynergy = false;
    public bool Dragging = false;
    private bool IsshopCard = false;
    private bool bigger = false;
    private bool Bigger
    {
        get => bigger;
        set
        {
            if(sizeChanger != null && bigger != value)
            {
                StopCoroutine(sizeChanger);
            }
            bigger = value;
        }
    }

    private GameObject synergySlotObj = null;

    private BattleManager battle;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        IsshopCard = transform.Find("Gold");
    }
    public void SetCardData(string id)
    {
        battle = GameManager.Inst.battle;
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
                cardImage.sprite = Resources.Load<Sprite>($"Images/Card/{data.category[0]}Card");
                DisplayData();
                return;
            }
        }
    }
    private IEnumerator BigSize()
    {
        Bigger = true;
        Vector3 size = IsshopCard ? new Vector3(2.85f, 2.85f, 1) : new Vector3(2f, 2f, 1);
        while((transform.GetChild(0).localScale - size).sqrMagnitude > 0.01f)
        {
            transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(0).localScale, size, Time.deltaTime * 20);
            yield return null;
        }
        transform.GetChild(0).localScale = size;
    }
    private IEnumerator SmallSize()
    {
        Bigger = false;
        Vector3 size = new Vector3(1f, 1f, 1);
        while ((transform.GetChild(0).localScale - size).sqrMagnitude > 0.01f)
        {
            transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(0).localScale, size, Time.deltaTime * 20);
            yield return null;
        }
        transform.GetChild(0).localScale = size;
    }
    private void DisplayData()
    {
        if(Gold != null)
        {
            Mana.text = data.mana.ToString();
            Gold.text = $"{data.gold} G";
            IsshopCard = true;
            cardImage.sprite = Resources.Load<Sprite>($"Images/Card/{data.category[0]}ShopCard");
        }
        else
        {
            Mana.text = data.mana.ToString();
        }
        Name.text = data.cardName;
        Desc.text = data.cardDesc;
        ReplaceDesc(null);
        IsUse = false;
        image.color = new (1.0f, 1.0f, 1.0f);
        cardImage.color = new(1.0f, 1.0f, 1.0f);
        img = null;
    }
    public void ReplaceDesc(Character _target)
    {
        if (data.cardDesc.IndexOf("{") > -1)
        {
            string convertDesc = "";
            string[] tempString = data.cardDesc.Split('{', '}');
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
                        else if(effItem[0] == "Self")
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
                                _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[4]), 0, Convert.ToInt32(effItem[3].Replace("t", "")), battle.Caster);
                            }
                            else if (effItem.Length == 4)
                            {
                                _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), Convert.ToInt32(effItem[3]), 0, 0, battle.Caster);
                            }
                        }
                        else
                        {
                            if (effItem.Length == 5)
                            {
                                _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, Mathf.RoundToInt(Convert.ToInt32(effItem[4]) * (1 - resist)), Convert.ToInt32(effItem[3].Replace("t", "")), battle.Caster);
                            }
                            else if (effItem.Length == 4)
                            {
                                if (effItem[3].IndexOf('t') > -1)
                                {
                                    _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, 0, Convert.ToInt32(effItem[3].Replace("t", "")), battle.Caster);
                                }
                                else
                                {
                                    Debug.Log($"{Convert.ToInt32(effItem[3]) * (1 - resist)} {(int)(Convert.ToInt32(effItem[3]) * (1 - resist))}");
                                    _eff = new((Enums.effType)Enum.Parse(typeof(Enums.effType), battle.FindEffType(effItem[1])), (Enums.eff)Enum.Parse(typeof(Enums.eff), effItem[1]), 0, Mathf.RoundToInt(Convert.ToInt32(effItem[3]) * (1 - resist)), 0, battle.Caster);
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
        if(target == null)
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
    public IEnumerator UseCard()
    {
        Vector2 pos;
        foreach(CardData card in FindObjectsOfType<CardData>())
        {
            card.ReplaceDesc(null);
        }
        if(battle.Caster.CompareTag("Player"))
        {
            pos = new Vector2(855f, 160f);
        }
        else
        {
            pos = new Vector2(960f, 540f);
            battle.UseCardUI.transform.SetAsLastSibling();
        }
        while ((rect.anchoredPosition - pos).sqrMagnitude > 0.01f)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, pos, Time.deltaTime * 10);
            yield return null;
        }
        rect.anchoredPosition = pos;

        useCard = null;
    }
    private IEnumerator DragEndCard()
    {
        foreach(CardData card in FindObjectsOfType<CardData>())
        {
            card.GetComponent<Image>().raycastTarget = true;
        }
        while (useCard!=null)
        {
            yield return null;
        }
        if (IsUse)
        {
            if(!DragSynergy)
                yield return new WaitForSeconds(1);
            image.color = new Color(0.5f, 0.5f, 0.5f);
            cardImage.color = new Color(0.5f, 0.5f, 0.5f);
        }
        while ((rect.anchoredPosition - startPoint).sqrMagnitude > 0.01f)
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
            Transform cardDetailBox = battle.CardUI.transform.parent.parent.parent.Find("CardDetailBox");
            cardDetailBox.transform.Find("Card").GetComponent<CardDetail>().SetCardData(id);
            cardDetailBox.SetAsLastSibling();
            cardDetailBox.gameObject.SetActive(true);
        }
        if (IsshopCard || Dragging || data.mana > battle.Mana)
            return;
        startPoint = rect.anchoredPosition;
        moveBegin = eventData.position;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsshopCard || data.mana > battle.Mana || IsUse)
            return;
        if (Array.IndexOf(data.category, "target") == -1)
        {
            Cursor.SetCursor(GameManager.Inst.CursorImg, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(GameManager.Inst.AttackCursor, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (IsshopCard || data.mana > battle.Mana || IsUse || IsUse)
            return;

        ReplaceDesc(null);
        canvasGroup.alpha = 1;
        battle.cardDragging = true;
        sizeChanger = StartCoroutine(BigSize());
        foreach (Character character in battle.order)
        {
            character.Targeting.SetActive(false);
            if(!character.isDead)
            {
                character.SubHP = character.HP;
            }
        }
        if (!IsUse)
        {
            Dragging = true;
            bool onSpawnable = false;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(eventData.position), transform.forward, 100.0f, LayerMask.GetMask("Field"));
            bool isTargeting = (hit.collider != null) && hit.collider.gameObject.CompareTag("Enemy") && (Array.IndexOf(data.category, "target")>-1);
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            if (Array.IndexOf(data.category, "target") == -1)
            {
                moveOffset = (eventData.position - moveBegin) * new Vector2(1920.0f / Screen.width, 1080.0f / Screen.height);
                rect.anchoredPosition = startPoint + moveOffset;
            }
            if (hit.collider != null && Array.IndexOf(data.category, "self") > - 1)
            {
                GameManager.Inst.player.Targeting.SetActive(true);
            }
            if (hit.collider != null && (Array.IndexOf(data.category, "enemies") > - 1 || Array.IndexOf(data.category, "entire") > - 1))
            {
                if(Array.IndexOf(data.category, "enemies") > -1)
                {
                    Enemy[] enemies = FindObjectsOfType<Enemy>();
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.GetComponent<BoxCollider2D>().enabled)
                            enemy.Targeting.SetActive(true);
                    }
                }
                if(Array.IndexOf(data.category, "entire") > -1)
                {
                    foreach (Character character in battle.order)
                    {
                        if (character.GetComponent<BoxCollider2D>().enabled)
                            character.Targeting.SetActive(true);
                    }
                }
            }
            
            if(isTargeting)
            {
                ReplaceDesc(hit.collider.GetComponent<Character>());
                hit.collider.GetComponent<Enemy>().Targeting.SetActive(true);
                battle.CalcSubDamage(hit.collider.GetComponent<Character>(), data.eff);
            }
            else if((hit.collider != null) && (Array.IndexOf(data.category, "target") == -1))
            {
                battle.CalcSubDamage(null, data.eff);
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
                    synergySlotObj = result.gameObject;
                    img.color = new Color(0.5f, 0.5f, 0.5f);
                    canvasGroup.alpha = 0;
                }
            }
            if (!onSpawnable && img != null && !synergySlotObj.GetComponent<SpawnSlotData>().FilledSlot)
            {
                img.gameObject.SetActive(false);
                img.sprite = null;
                img.color = Color.white;
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsshopCard || data.mana > battle.Mana || IsUse)
            return;
        battle.cardDragging = false;
        Cursor.visible = true;
        Cursor.SetCursor(GameManager.Inst.CursorImg, Vector2.zero, CursorMode.ForceSoftware);
        sizeChanger = StartCoroutine(SmallSize());
        if (!IsUse)
        {
            foreach (Character character in battle.order)
            {
                character.Targeting.SetActive(false);
            }
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);
            canvasGroup.alpha = 1;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(eventData.position), transform.forward, 100.0f, LayerMask.GetMask("Field"));
            bool isTargeting = (hit.collider != null) && hit.collider.gameObject.CompareTag("Enemy") && (Array.IndexOf(data.category, "target") > -1);
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
                    IsUse = true;
                    battle.Mana -= data.mana;
                    DragSynergy = true;
                }
            }
            if(isTargeting)
            {
                IsUse = true;
                battle.Target = hit.collider.GetComponent<Character>();
                battle.SetEff(data.eff);
                battle.Mana -= data.mana;
                useCard = StartCoroutine(UseCard());
            }
            if(hit.collider != null && Array.IndexOf(data.category, "target") == -1)
            {
                IsUse = true;
                image.color = new Color(0.5f, 0.5f, 0.5f);
                cardImage.color = new Color(0.5f, 0.5f, 0.5f);
                battle.SetEff(data.eff);
                battle.Mana -= data.mana;
                useCard = StartCoroutine(UseCard());
            }
        }
        StartCoroutine(DragEndCard());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsshopCard && !IsUse)
        {
            if(data.gold <= GameManager.Inst.player.Gold)
            {
                GameManager.Inst.player.Gold -= data.gold;
                GameManager.Inst.player.AddCard(id);
                image.color = new Color(0.5f, 0.5f, 0.5f);
                cardImage.color = new Color(0.5f, 0.5f, 0.5f);
                IsUse = true;
                sizeChanger = StartCoroutine(SmallSize());
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!IsUse && !battle.cardDragging)
        {
            transform.parent.parent.SetAsLastSibling();
            if (IsshopCard)
            {
                transform.parent.parent.SetAsLastSibling();
                if (rect.anchoredPosition.x == 855)
                {
                    transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 0);
                }
                if (rect.anchoredPosition.y == -260)
                {
                    transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
                }
            }
            sizeChanger = StartCoroutine(BigSize());
            transform.SetAsLastSibling();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsUse)
        {
            sizeChanger = StartCoroutine(SmallSize());
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
}