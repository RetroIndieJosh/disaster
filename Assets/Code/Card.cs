using System.Collections;
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
public class Card : GameElement
{
    public Player Owner {
        private get; set;
    }

    public UnityEvent m_onPlayed = new UnityEvent();

    Button m_button = null;
    private CardType m_type = CardType.None;
    private bool m_isCardActive = false;

    public bool IsCardActive {
        get => m_isCardActive;
        set {
            var rectTrans = GetComponent<RectTransform>();
            var height = rectTrans.rect.height / 2;
            var pos = rectTrans.anchoredPosition;
            m_isCardActive = value;
            Debug.Log($"Set card {name} " + (m_isCardActive ? "active" : "inactive"));
            pos.y = m_isCardActive ? Mathf.FloorToInt(height) / 2 : 0;
            rectTrans.anchoredPosition = pos;

            if (m_isCardActive) {
                Board.instance.ToggleTiles((t) => {
                    return t.HasAdjacentOrthogonalStone(Owner) && t.State == BoardTileState.Clear;
                });
            } else
                Board.instance.ResetTiles();
        }
    }

    public CardType CardType {
        get => m_type;
        set {
            m_type = value;
            UpdateColor();
            UpdateInfo();
        }
    }

    public virtual void Play(BoardTile a_tile) {
        if (m_type == CardType.Life) {
            a_tile.SetStone(Owner);
        }
        Board.instance.ActiveCard = null;
        CardType = CardType.None;
        Owner.PlayedCard();
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void Start() {
        UpdateInfo();

        m_button.onClick.AddListener(() => {
            Board.instance.ActiveCard = this;
        });
    }

    private void UpdateColor() {
        gameObject.SetActive(true);
        var colors = m_button.colors;
        switch (m_type) {
        case CardType.Fire:
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.blue;
            break;
        case CardType.Life:
            colors.normalColor = Color.green;
            break;
        case CardType.Move:
            colors.normalColor = Color.black;
            break;
        case CardType.None:
            gameObject.SetActive(false);
            colors.normalColor = Color.white;
            break;
        case CardType.Plague:
            colors.normalColor = Color.yellow;
            break;
        case CardType.Spread:
            colors.normalColor = Color.gray;
            break;
        case CardType.Step:
            colors.normalColor = Color.white;
            break;
        case CardType.Water:
            colors.normalColor = Color.blue;
            break;
        case CardType.Wild:
            colors.normalColor = new Color(0.8f, 0.0f, 0.5f);
            break;
        }
        m_button.colors = colors;
    }

    private void UpdateInfo() {
        m_infoText = $"{Owner.name} Card: ";
        switch (m_type) {
        /*
        case CardType.Fire:
            m_infoText = "Fire";
            break;
        case CardType.Life:
            m_infoText = "Fire";
            break;
        case CardType.Move:
            m_infoText = "Fire";
            break;
        case CardType.None:
            m_infoText = "Fire";
            break;
        case CardType.Plague:
            m_infoText = "Fire";
            break;
        case CardType.Spread:
            m_infoText = "Fire";
            break;
        case CardType.Step:
            m_infoText = "Fire";
            break;
        case CardType.Water:
            m_infoText = "Fire";
            break;
        case CardType.Wild:
            m_infoText = "Fire";
            break;
            */
        default:
            m_infoText += m_type.ToString();
            break;
        }
    }
}
