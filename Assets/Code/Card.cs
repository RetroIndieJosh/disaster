﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum CardType
{
    None = 0x00,
    Move = 0x01, // move up to two
    Life = 0x02, // add one
    Step = 0x04, // advance one
    Spread = 0x08, // advance two
    Fire = 0x0F,
    Water = 0x10,
    Plague = 0x11,
    Wild = 0x12,
    Reserved1 = 0x14,
    Reserved2 = 0x18,
    Reserved3 = 0x1F
}

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    public Player Owner {
        private get; set;
    }

    public UnityEvent m_onPlayed = new UnityEvent();

    Button m_button = null;
    private CardType m_type = CardType.None;
    private bool m_isActive = false;

    public bool IsActive {
        get => m_isActive;
        set {
            var rectTrans = GetComponent<RectTransform>();
            var height = rectTrans.rect.height / 2;
            var pos = rectTrans.anchoredPosition;
            m_isActive = value;
            Debug.Log($"Set card {name} " + (m_isActive ? "active" : "inactive"));
            pos.y = m_isActive ? Mathf.FloorToInt(height) / 2 : 0;
            rectTrans.anchoredPosition = pos;
        }
    }

    public CardType CardType {
        get => m_type;
        set {
            m_type = value;
            UpdateColor();
        }
    }

    public virtual void Play(BoardTile a_tile) {
        a_tile.SetStone(Owner);
        m_type = CardType.None;
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void Start() {
        m_button.onClick.AddListener(() => {
            TurnManager.instance.ActiveCard = this;
        });
    }

    private void UpdateColor() {
        var colors = m_button.colors;
        switch (m_type) {
        case CardType.Fire:
            colors.normalColor = Color.red;
            break;
        case CardType.Life:
            colors.normalColor = Color.green;
            break;
        case CardType.Move:
            colors.normalColor = Color.black;
            break;
        case CardType.None:
            colors.normalColor = Color.white;
            break;
        case CardType.Plague:
            colors.normalColor = Color.yellow;
            break;
        case CardType.Spread:
            colors.normalColor = Color.gray;
            break;
        case CardType.Step:
            colors.normalColor = Color.gray;
            break;
        case CardType.Water:
            colors.normalColor = Color.blue;
            break;
        case CardType.Wild:
            colors.normalColor = new Color(0.8f, 0.0f, 0.5f);
            break;
        }
        m_button.colors = colors;
        Debug.Log($"Set card {CardType} color to {colors.normalColor}");
    }
}
