using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected string m_infoText = "";

    public void OnPointerEnter(PointerEventData eventData) {
        Board.instance.InfoText = m_infoText;
    }

    public void OnPointerExit(PointerEventData eventData) {
        Board.instance.InfoText = "";
    }
}
