using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SynergySlotData : MonoBehaviour, IPointerClickHandler
{
    public Image illust;
    public Card data;
    public bool FilledSlot = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            foreach(string synergy in data.synergy)
            {
                GameManager.Inst.battle.synergies.Remove(synergy);
            }
            data = null;
            illust.sprite = null;
            illust.gameObject.SetActive(false);
            FilledSlot = false;
        }
    }
}
